import { readdirSync, readFileSync, writeFileSync, statSync } from 'fs';
import { join } from 'path';

const POSTS_DIR = join(process.cwd(), 'public', 'posts');
const OUT_FILE = join(POSTS_DIR, 'manifest.json');

const entries = readdirSync(POSTS_DIR).filter(name => {
  const full = join(POSTS_DIR, name);
  return statSync(full).isDirectory();
});

const manifest = entries
  .map(slug => {
    const metaPath = join(POSTS_DIR, slug, 'meta.json');
    try {
      const meta = JSON.parse(readFileSync(metaPath, 'utf-8'));
      return { ...meta, slug: meta.slug || slug };
    } catch {
      console.warn(`Skipping "${slug}" -- no valid meta.json`);
      return null;
    }
  })
  .filter(Boolean)
  .sort((a, b) => new Date(b.date) - new Date(a.date));

writeFileSync(OUT_FILE, JSON.stringify(manifest, null, 2));
console.log(`Wrote ${manifest.length} posts to manifest.json`);