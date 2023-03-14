using System.Runtime.CompilerServices;
using FollowYourDreams;

[assembly: InternalsVisibleTo(AssemblyInfo.NAMESPACE_EDITOR)]
[assembly: InternalsVisibleTo(AssemblyInfo.NAMESPACE_TESTS)]

namespace FollowYourDreams {
    public static class AssemblyInfo {
        public const string NAMESPACE_RUNTIME = "FollowYourDreams";
        public const string NAMESPACE_EDITOR = "FollowYourDreams.Editor";
        public const string NAMESPACE_INPUT = "FollowYourDreams.Input";

        public const string NAMESPACE_TESTS = "FollowYourDreams.Tests";

        public const string MENU = "Follow Your Dreams/";
        public const string MENU_LEVEL_TILE = "Follow Your Dreams/Tiles/";
    }
}