#!/usr/bin/env node
/**
 * Post-traitement esthétique standard Katana UI :
 * - icon      : fond sombre retiré + crop (armes, objets)
 * - frame     : letterbox retiré + crop (grilles, barres)
 * - frameThin : idem + centre percé (cadre fenêtre fin, centre transparent)
 * - overlay   : letterbox retiré (ornements avant-plan)
 */

const fs = require("fs");
const path = require("path");
const sharp = require("sharp");

const projectRoot = path.resolve(__dirname, "..", "..");
const uiRoot = path.join(projectRoot, "Assets", "_Project", "Resources", "UI");
const assetsRoot = path.join(
  process.env.USERPROFILE || "",
  ".cursor",
  "projects",
  "c-Users-Philippe-Documents-Unity-Projects-Katana",
  "assets"
);
const dryRun = process.argv.includes("--dry-run");

const sourceCopies = [
  {
    from: path.join(assetsRoot, "window_frame_thin.png"),
    to: path.join(uiRoot, "Sprites", "window_frame.png"),
  },
  {
    from: path.join(assetsRoot, "window_frame_overlay_small.png"),
    to: path.join(uiRoot, "Sprites", "window_frame_overlay.png"),
  },
];

const jobs = [
  {
    file: path.join(uiRoot, "Weapons", "katana_icon.png"),
    mode: "icon",
    borderRatio: 0,
  },
  {
    file: path.join(uiRoot, "Sprites", "window_frame.png"),
    mode: "frameThin",
    borderRatio: 0.08,
  },
  {
    file: path.join(uiRoot, "Sprites", "window_frame_overlay.png"),
    mode: "overlay",
    borderRatio: 0.08,
    borderFn: (width, height, job) => {
      const frameMeta = path.join(uiRoot, "Sprites", "window_frame.png.meta");
      if (!fs.existsSync(frameMeta)) return null;
      const text = fs.readFileSync(frameMeta, "utf8");
      const match = text.match(/spriteBorder:\s*\{x:\s*(\d+),\s*y:\s*(\d+),\s*z:\s*(\d+),\s*w:\s*(\d+)\}/);
      if (!match) return null;
      return {
        left: Number(match[1]),
        bottom: Number(match[2]),
        right: Number(match[3]),
        top: Number(match[4]),
      };
    },
  },
  {
    file: path.join(uiRoot, "Sprites", "grid_frame.png"),
    mode: "frame",
    borderRatio: 0.14,
  },
  {
    file: path.join(uiRoot, "Sprites", "health_bar_bg.png"),
    mode: "frame",
    borderRatio: 0.12,
    borderFn: (width, height) => ({
      left: Math.round(Math.min(width * 0.085, 140)),
      bottom: Math.round(Math.max(height * 0.18, 12)),
      right: Math.round(Math.min(width * 0.085, 140)),
      top: Math.round(Math.max(height * 0.18, 12)),
    }),
  },
];

const threshold = 48;

function luminance(r, g, b) {
  return 0.299 * r + 0.587 * g + 0.114 * b;
}

function isLetterbox(r, g, b) {
  const lum = luminance(r, g, b);
  return r <= threshold && g <= threshold && b <= threshold && lum <= threshold + 6;
}

function floodBackgroundFromEdges(data, width, height, channels) {
  const visited = new Uint8Array(width * height);
  const queue = [];

  function tryAdd(x, y) {
    if (x < 0 || y < 0 || x >= width || y >= height) return;
    const idx = y * width + x;
    if (visited[idx]) return;
    const i = idx * channels;
    if (!isLetterbox(data[i], data[i + 1], data[i + 2])) return;
    visited[idx] = 1;
    queue.push(idx);
  }

  for (let x = 0; x < width; x++) {
    tryAdd(x, 0);
    tryAdd(x, height - 1);
  }
  for (let y = 0; y < height; y++) {
    tryAdd(0, y);
    tryAdd(width - 1, y);
  }

  while (queue.length > 0) {
    const idx = queue.pop();
    const i = idx * channels;
    data[i + 3] = 0;
    const x = idx % width;
    const y = (idx / width) | 0;
    tryAdd(x + 1, y);
    tryAdd(x - 1, y);
    tryAdd(x, y + 1);
    tryAdd(x, y - 1);
  }
}

function punchCenterFill(data, width, height, channels, marginRatio = 0.17, maxLum = 78) {
  const minX = Math.floor(width * marginRatio);
  const maxX = Math.floor(width * (1 - marginRatio));
  const minY = Math.floor(height * marginRatio);
  const maxY = Math.floor(height * (1 - marginRatio));

  for (let y = minY; y <= maxY; y++) {
    for (let x = minX; x <= maxX; x++) {
      const i = (y * width + x) * channels;
      const lum = luminance(data[i], data[i + 1], data[i + 2]);
      if (lum <= maxLum) data[i + 3] = 0;
    }
  }
}

function removeIconBackground(data, width, height, channels) {
  for (let y = 0; y < height; y++) {
    for (let x = 0; x < width; x++) {
      const i = (y * width + x) * channels;
      if (isLetterbox(data[i], data[i + 1], data[i + 2])) data[i + 3] = 0;
    }
  }
}

function cropBounds(data, width, height, channels, pad = 8) {
  let minX = width;
  let minY = height;
  let maxX = 0;
  let maxY = 0;

  for (let y = 0; y < height; y++) {
    for (let x = 0; x < width; x++) {
      const i = (y * width + x) * channels;
      if (data[i + 3] <= 0) continue;
      if (x < minX) minX = x;
      if (y < minY) minY = y;
      if (x > maxX) maxX = x;
      if (y > maxY) maxY = y;
    }
  }

  if (maxX < minX || maxY < minY) {
    throw new Error("No opaque pixels left after background removal.");
  }

  minX = Math.max(0, minX - pad);
  minY = Math.max(0, minY - pad);
  maxX = Math.min(width - 1, maxX + pad);
  maxY = Math.min(height - 1, maxY + pad);

  return {
    left: minX,
    top: minY,
    width: maxX - minX + 1,
    height: maxY - minY + 1,
  };
}

function computeBorder(width, height, ratio, borderFn, job) {
  if (borderFn) return borderFn(width, height, job);
  if (ratio <= 0) return null;
  const base = Math.round(Math.min(width, height) * ratio);
  return { left: base, bottom: base, right: base, top: base };
}

function updateMetaBorder(metaPath, border) {
  if (!border || !fs.existsSync(metaPath)) return;
  let text = fs.readFileSync(metaPath, "utf8");
  const line = `  spriteBorder: {x: ${border.left}, y: ${border.bottom}, z: ${border.right}, w: ${border.top}}`;
  if (/spriteBorder:\s*\{[^}]+\}/.test(text)) {
    text = text.replace(/spriteBorder:\s*\{[^}]+\}/, line.trim());
  } else {
    text = text.replace(/spritePixelsToUnits:\s*\d+/, (match) => `${match}\n  ${line.trim()}`);
  }
  fs.writeFileSync(metaPath, text, "utf8");
}

function copySources() {
  for (const copy of sourceCopies) {
    if (!fs.existsSync(copy.from)) {
      console.warn(`SKIP source missing: ${copy.from}`);
      continue;
    }
    fs.mkdirSync(path.dirname(copy.to), { recursive: true });
    fs.copyFileSync(copy.from, copy.to);
    console.log(`Copied ${path.relative(projectRoot, copy.to)}`);
  }
}

async function processJob(job) {
  if (!fs.existsSync(job.file)) {
    console.warn(`SKIP missing: ${job.file}`);
    return;
  }

  const { data, info } = await sharp(job.file).ensureAlpha().raw().toBuffer({ resolveWithObject: true });
  const { width, height, channels } = info;

  if (job.mode === "icon") {
    removeIconBackground(data, width, height, channels);
  } else {
    floodBackgroundFromEdges(data, width, height, channels);
    if (job.mode === "frameThin") punchCenterFill(data, width, height, channels, 0.17, 78);
  }

  const crop = cropBounds(data, width, height, channels, job.mode === "icon" ? 8 : 4);
  const border = computeBorder(crop.width, crop.height, job.borderRatio, job.borderFn, job);
  const rel = path.relative(projectRoot, job.file);

  console.log(
    `${rel}: ${width}x${height} -> ${crop.width}x${crop.height}` +
      (border ? ` | 9-slice ${border.left}px` : "")
  );

  if (dryRun) return;

  await sharp(data, { raw: { width, height, channels } })
    .extract(crop)
    .png()
    .toFile(job.file);

  updateMetaBorder(`${job.file}.meta`, border);
}

async function main() {
  console.log(`Katana UI sprite post-process${dryRun ? " (dry-run)" : ""}`);
  copySources();
  for (const job of jobs) await processJob(job);
  console.log("Done.");
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
