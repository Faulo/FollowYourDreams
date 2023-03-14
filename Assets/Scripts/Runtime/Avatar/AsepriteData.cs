using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace FollowYourDreams.Avatar {
    [Serializable]
    class AsepriteData {
        public static AsepriteData FromJson(string text) {
            return JsonConvert.DeserializeObject<AsepriteData>(text);
        }
        [SerializeField]
        public Dictionary<string, AsepriteDataFrame> frames = new();
        [SerializeField]
        public AsepriteDataMeta meta = new();
    }
    [Serializable]
    class AsepriteDataFrame {
        public AsepriteDataSize sourceSize = new();
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
        public string direction;
    }
}
