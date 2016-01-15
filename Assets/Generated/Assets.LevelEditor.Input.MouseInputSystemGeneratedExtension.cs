namespace Entitas {
    public partial class Pool {
        public ISystem CreateMouseInputSystem() {
            return this.CreateSystem<Assets.LevelEditor.Input.MouseInputSystem>();
        }
    }
}