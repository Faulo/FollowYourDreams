using FollowYourDreams.Avatar;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace FollowYourDreams.Tests {
    public sealed class TestAseprite {
        string data;

        [SetUp]
        public void SetUp() {
            data = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Scripts/Tests/TestAseprite.json").text;
        }

        [Test]
        public void TestAsepriteSimplePasses() {
            Assert.IsNotNull(data);
        }

        [Test]
        public void TestFromJson() {
            Assert.IsNotNull(AsepriteData.FromJson(data));
        }

        [Test]
        public void TestFromJsonFrames() {
            var obj = AsepriteData.FromJson(data);
            Assert.IsNotNull(obj.frames);
            Assert.Greater(obj.frames.Length, 0);
            Assert.IsNotNull(obj.frames[0]);

            var frame = obj.frames[0];

            Assert.AreEqual("S_ENT_Avatar 0.aseprite", frame.filename);
            Assert.AreEqual(0, frame.frame.x);
            Assert.AreEqual(0, frame.frame.y);
            Assert.AreEqual(160, frame.frame.w);
            Assert.AreEqual(32, frame.frame.h);
            Assert.AreEqual(160, frame.sourceSize.w);
            Assert.AreEqual(32, frame.sourceSize.h);
            Assert.AreEqual(3500, frame.duration);
            Assert.AreEqual(false, frame.rotated);
            Assert.AreEqual(false, frame.trimmed);
            Assert.AreEqual(0, frame.spriteSourceSize.x);
            Assert.AreEqual(0, frame.spriteSourceSize.y);
            Assert.AreEqual(160, frame.spriteSourceSize.w);
            Assert.AreEqual(32, frame.spriteSourceSize.h);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void TestThis(int id) {
            var obj = AsepriteData.FromJson(data);
            Assert.AreEqual(obj.frames[id], obj[$"S_ENT_Avatar {id}.aseprite"]);
        }
        [Test]
        public void TestFromJsonMeta() {
            var obj = AsepriteData.FromJson(data);
            Assert.IsNotNull(obj.meta);
            Assert.AreEqual("https://www.aseprite.org/", obj.meta.app);
            Assert.AreEqual("1.2.40-x64", obj.meta.version);
            Assert.AreEqual("I8", obj.meta.format);
            Assert.AreEqual(80, obj.meta.size.w);
            Assert.AreEqual(192, obj.meta.size.h);
            Assert.AreEqual(1, obj.meta.scale);
        }

        [Test]
        public void TestFromJsonMetaLayers() {
            var obj = AsepriteData.FromJson(data);
            Assert.IsNotNull(obj.meta.layers);
            Assert.AreEqual(1, obj.meta.layers.Length);
            Assert.IsNotNull(obj.meta.layers[0]);
            Assert.AreEqual("avatar", obj.meta.layers[0].name);
            Assert.AreEqual(255, obj.meta.layers[0].opacity);
            Assert.AreEqual("normal", obj.meta.layers[0].blendMode);
        }

        [Test]
        public void TestFromJsonMetaFrameTags() {
            var obj = AsepriteData.FromJson(data);
            Assert.IsNotNull(obj.meta.frameTags);
            Assert.AreEqual(3, obj.meta.frameTags.Length);
            Assert.IsNotNull(obj.meta.frameTags[0]);
            Assert.AreEqual("idle", obj.meta.frameTags[0].name);
            Assert.AreEqual(0, obj.meta.frameTags[0].from);
            Assert.AreEqual(1, obj.meta.frameTags[0].to);
            Assert.AreEqual(AsepriteDataFrameDirection.forward, obj.meta.frameTags[0].direction);
        }
    }
}