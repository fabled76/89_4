"""Read-only geometry/image verification; writes verification.json and SHA256SUMS.txt.
Run with Python 3 from any directory. No Blender or third-party module required.
"""
import hashlib
import json
import math
from pathlib import Path
import struct

root = Path(__file__).resolve().parent
manifest = json.loads((root / "camera_layout_manifest.json").read_text(encoding="utf-8"))
vertices, normals, faces, groups = [], [], [], {}
current = None
for line in (root / "blockout_geometry.obj").read_text(encoding="utf-8").splitlines():
    fields = line.split()
    if not fields:
        continue
    if fields[0] == "o":
        current = fields[1]
        assert current not in groups
        groups[current] = []
    elif fields[0] == "v":
        value = tuple(map(float, fields[1:]))
        assert len(value) == 3 and all(map(math.isfinite, value))
        vertices.append(value)
        groups[current].append(value)
    elif fields[0] == "vn":
        normals.append(tuple(map(float, fields[1:])))
    elif fields[0] == "f":
        faces.append([tuple(int(value) - 1 for value in field.split("/")) for field in fields[1:]])

assert len(groups) == len(manifest["meshObjects"]) == 49
assert len(normals) == len(vertices)
for triangle in faces:
    assert len(triangle) == 3
    for indexes in triangle:
        assert all(0 <= value < len(vertices) for value in indexes)
    a, b, c = [vertices[indexes[0]] for indexes in triangle]
    ab = [b[i] - a[i] for i in range(3)]
    ac = [c[i] - a[i] for i in range(3)]
    cross = (ab[1]*ac[2]-ab[2]*ac[1], ab[2]*ac[0]-ab[0]*ac[2], ab[0]*ac[1]-ab[1]*ac[0])
    normal = normals[triangle[0][2]]
    assert sum(cross[i]*normal[i] for i in range(3)) > 0, "Mirrored OBJ winding must agree with normal"

def near(actual, expected, label):
    assert len(actual) == len(expected)
    assert max(abs(a-b) for a, b in zip(actual, expected)) < .0001, (label, actual, expected)

for item in manifest["meshObjects"]:
    parent = item["hierarchy"].split("/")[-2]
    points = groups[parent + "__" + item["name"]]
    # Convert baked OBJ vertices back to Unity world coordinates.
    points = [(p[0], p[1], -p[2]) for p in points]
    minimum = [min(p[i] for p in points) for i in range(3)]
    maximum = [max(p[i] for p in points) for i in range(3)]
    near(minimum, item["boundsMin"], item["name"])
    near(maximum, item["boundsMax"], item["name"])
    near(item["localScale"], [1, 1, 1], item["name"])

objects = {item["name"]: item for item in manifest["objects"]}
near(objects["ReferencePerson"]["dimensionsXYZ"][1:2], [1.8], "figure height")
near(objects["TravelBag"]["dimensionsXYZ"], [.45, .30, .25], "bag XYZ includes handle")
mesh = {item["name"]: item for item in manifest["meshObjects"]}
for name, expected in {
    "Calibration_1m_Cube": [1, 1, 1], "Door_Leaf": [.85, 2.1, .04],
    "Desk_Top": [1.35, .04, .65], "Bed_Mattress": [1.95, .18, .90],
    "Receiver_Body": [.18, .11, .05],
}.items():
    near(mesh[name]["dimensionsXYZ"], expected, name)

camera = manifest["camera"]
near([camera["projectedWidth"], camera["projectedHeight"]], [8, 4.5], "camera span")
for anchor in manifest["anchors"]:
    delta = [anchor["unityWorld"][i] - camera["position"][i] for i in range(3)]
    x = sum(delta[i]*camera["right"][i] for i in range(3))
    y = sum(delta[i]*camera["up"][i] for i in range(3))
    pixel = [(x/8 + .5)*1920, (.5 - y/4.5)*1080]
    assert max(abs(a-b) for a,b in zip(pixel, anchor["pixelTopLeft"])) < .002

images = {}
for path in sorted(root.rglob("*.png")):
    data = path.read_bytes()
    assert data[:8] == b"\x89PNG\r\n\x1a\n"
    width, height = struct.unpack(">II", data[16:24])
    expected = (960, 540) if "960" in path.name else (1280, 720) if "1280" in path.name else (1920, 1080)
    assert (width, height) == expected
    images[str(path.relative_to(root))] = [width, height]
assert len(images) == 11
report = {
    "result": "PASS", "meshObjects": len(groups), "vertices": len(vertices), "triangles": len(faces),
    "checks": ["OBJ indices, finite vertices, winding/normals", "all 49 mesh bounds agree with Unity manifest",
               "unit transform scales and calibration cube", "declared reference dimensions", "camera anchor reprojection", "PNG headers and sizes"],
    "images": images,
    "limits": "PNG content reviewed separately by programmer. No Blender import/render or final art approval claimed.",
}
(root / "verification.json").write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
paths = sorted(path for path in root.rglob("*") if path.is_file() and path.name != "SHA256SUMS.txt")
(root / "SHA256SUMS.txt").write_text("".join(hashlib.sha256(path.read_bytes()).hexdigest() + "  " + path.relative_to(root).as_posix() + "\n" for path in paths), encoding="utf-8")
print(json.dumps({key: report[key] for key in ("result", "meshObjects", "vertices", "triangles")}))
