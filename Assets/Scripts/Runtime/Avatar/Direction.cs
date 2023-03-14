using UnityEngine;

namespace FollowYourDreams.Avatar {
    enum Direction {
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft
    }
    static class DirectionExtensions {
        public static void Set(this ref Direction direction, float angle) {
            // Normalize the angle to be between 0 and 360 degrees
            angle = (angle + 360f) % 360f;

            // Calculate the index of the closest direction
            int index = Mathf.RoundToInt(angle / 45f) % 8;

            // Convert the index to a Direction enum value
            direction = (Direction)index;
        }
    }
}
