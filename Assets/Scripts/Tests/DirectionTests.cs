using FollowYourDreams.Avatar;
using NUnit.Framework;

namespace FollowYourDreams.Tests {
    public sealed class DirectionTests {
        [TestCase(0, Direction.Up)]
        [TestCase(1080, Direction.Up)]
        [TestCase(-1080, Direction.Up)]
        [TestCase(30, Direction.UpRight)]
        [TestCase(90, Direction.Right)]
        [TestCase(135, Direction.DownRight)]
        [TestCase(180, Direction.Down)]
        [TestCase(210, Direction.DownLeft)]
        [TestCase(-90, Direction.Left)]
        [TestCase(690, Direction.UpLeft)]
        public void TestDirectionSet(float angle, Direction expected) {
            Direction direction = default;
            direction.Set(angle);
            Assert.AreEqual(expected, direction);
        }
    }
}
