import json, os, re

CHAPTER_PATH = r"D:\project\pintu\client\Assets\data\tbchapter.json"
LEVEL_PATH = r"D:\project\pintu\client\Assets\data\tblevel.json"
OUT_DIR = r"D:\project\pintu\client\Assets\StreamingAssets\galleries\default"
OUT_PATH = os.path.join(OUT_DIR, "gallery.json")

def extract_filename(url):
    m = re.search(r'pintu_res@main/(.+)$', url)
    return m.group(1) if m else url.split('/')[-1]

chapters = json.load(open(CHAPTER_PATH, 'r', encoding='utf-8'))
levels = json.load(open(LEVEL_PATH, 'r', encoding='utf-8'))

gallery = {
    "id": "default",
    "name": "默认风景图库",
    "theme": "自然风光",
    "description": "100张AI生成的风景画，包含日落、森林、草原、海岸等多种主题",
    "version": 1,
    "cover": "landscape_001.png",
    "cdn": {
        "provider": "jsdelivr",
        "owner": "twtwgetg",
        "repo": "pintu_res@main"
    },
    "chapters": [],
    "levels": []
}

for ch in chapters:
    gallery["chapters"].append({
        "id": ch["id"],
        "title": ch["chapter_title"],
        "figure": extract_filename(ch["chapter_figure"]),
        "cols": ch["chapter_figure_x"],
        "rows": ch["chapter_figure_y"],
        "nextChapter": ch["next_chapter"],
        "levels": ch["level_id"]
    })

for lv in levels:
    entry = {
        "id": lv["id"],
        "figure": extract_filename(lv["level_figure"]),
        "cols": lv["level_figure_x"],
        "rows": lv["level_figure_y"],
        "difficulty": lv["difficulty_tier"],
        "outOfPlace": lv["out_of_place_number"],
        "nextLevel": lv["next_level"]
    }
    if "m_distance_range" in lv and len(lv["m_distance_range"]) > 0:
        entry["distRange"] = lv["m_distance_range"]
    gallery["levels"].append(entry)

os.makedirs(OUT_DIR, exist_ok=True)
with open(OUT_PATH, 'w', encoding='utf-8') as f:
    json.dump(gallery, f, ensure_ascii=False, indent=2)

print(f"Generated: {OUT_PATH}")
print(f"  Chapters: {len(gallery['chapters'])}")
print(f"  Levels: {len(gallery['levels'])}")
