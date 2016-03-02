using Entitas;

namespace Entitas {
    public partial class Entity {
        static readonly Assets.LevelEditor.PreviewComponent previewComponent = new Assets.LevelEditor.PreviewComponent();

        public bool isPreview {
            get { return HasComponent(GameComponentIds.Preview); }
            set {
                if (value != isPreview) {
                    if (value) {
                        AddComponent(GameComponentIds.Preview, previewComponent);
                    } else {
                        RemoveComponent(GameComponentIds.Preview);
                    }
                }
            }
        }

        public Entity IsPreview(bool value) {
            isPreview = value;
            return this;
        }
    }
}

    public partial class GameMatcher {
        static IMatcher _matcherPreview;

        public static IMatcher Preview {
            get {
                if (_matcherPreview == null) {
                    var matcher = (Matcher)Matcher.AllOf(GameComponentIds.Preview);
                    matcher.componentNames = GameComponentIds.componentNames;
                    _matcherPreview = matcher;
                }

                return _matcherPreview;
            }
        }
    }

    public partial class UiMatcher {
        static IMatcher _matcherPreview;

        public static IMatcher Preview {
            get {
                if (_matcherPreview == null) {
                    var matcher = (Matcher)Matcher.AllOf(GameComponentIds.Preview);
                    matcher.componentNames = GameComponentIds.componentNames;
                    _matcherPreview = matcher;
                }

                return _matcherPreview;
            }
        }
    }
