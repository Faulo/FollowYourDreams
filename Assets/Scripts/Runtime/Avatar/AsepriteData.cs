using System;
using Newtonsoft.Json;
using UnityEngine;

namespace FollowYourDreams.Avatar {
    [Serializable]
    class AsepriteData {
        public static AsepriteData FromJson(string text) {
            return JsonConvert.DeserializeObject<AsepriteData>(text);
        }
        public AsepriteDataFrame this[string frameName] {
            get {
                for (int i = 0; i < frames.Length; i++) {
                    if (frames[i].filename == frameName) {
                        return frames[i];
                    }
                }
                throw new ArgumentOutOfRangeException(frameName);
            }
        }
        [SerializeField]
        public AsepriteDataFrame[] frames = Array.Empty<AsepriteDataFrame>();
        [SerializeField]
        public AsepriteDataMeta meta = new();
    }
    [Serializable]
    class AsepriteDataFrame {
        public string filename;
        public AsepriteDataRect frame = new();
        public bool rotated;
        public bool trimmed;
        public AsepriteDataRect spriteSourceSize = new();
        public AsepriteDataSize sourceSize = new();
        public int duration;
    }
    [Serializable]
    class AsepriteDataMeta {
        public string app;
        public string version;
        public string image;
        public string format;
        public AsepriteDataSize size = new();
        public int scale;
        public AsepriteDataFrameTag[] frameTags = Array.Empty<AsepriteDataFrameTag>();
        public AsepriteDataLayer[] layers = Array.Empty<AsepriteDataLayer>();
        public object slices;
    }
    [Serializable]
    class AsepriteDataSize {
        public int w;
        public int h;
        public override string ToString() => new Vector2Int(w, h).ToString();
    }
    [Serializable]
    class AsepriteDataRect {
        public int x;
        public int y;
        public int w;
        public int h;

        public Rect AsRect() => new(x, y, w, h);
        public RectInt AsRectInt() => new(x, y, w, h);

        public override string ToString() => AsRectInt().ToString();
    }
    [Serializable]
    class AsepriteDataLayer {
        public string name;
        public int opacity;
        public string blendMode;
    }
    [Serializable]
    class AsepriteDataFrameTag {
        public string name;
        public int from;
        public int to;
        public AsepriteDataFrameDirection direction;
    }
    enum AsepriteDataFrameDirection {
        forward,
        pingpong,
        reverse
    }
}
