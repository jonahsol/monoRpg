using Microsoft.Xna.Framework;

using Eid = System.UInt16;

namespace EntityComponentSystem
{
    /// <summary>
    /// Camera system updates and provides access to <see cref="CurTransformMatrix"/>.
    /// This is passed to Monogame call <see cref="SpriteBatch.Begin())"/>, manipulating game
    /// viewport.
    /// </summary>
    public static class CameraSystem
    {
        // camera follows a entity - set in <see cref="CameraComponent"> constructor
        public static Eid? CurCameraEid { get; set; }

        // Entity with camera component should have position and movement components also
        private static CameraComponent _cameraComponent;

        /// <summary>
        /// Returns transform matrix determined by <see cref="_cameraComponent"/> data. Current
        /// '_cameraComponent' is a component of entity <see cref="CurCameraEid"/>.
        /// </summary>
        public static Matrix CurTransformMatrix
        {
            get
            {
                if (_cameraComponent != null)
                {
                    return Matrix.Identity
                            *
                            Matrix.CreateTranslation(_cameraComponent.Origin.X,
                                                                    _cameraComponent.Origin.Y, 0)
                            *
                            Matrix.CreateTranslation(-_cameraComponent.Position.X,
                                                                -_cameraComponent.Position.Y, 0)
                            *
                            Matrix.CreateScale(_cameraComponent.Scale)
                            ;
                }
                else
                {
                    return Matrix.Identity;
                }
            }
        }

        public static Vector3 CameraPosition
        {
            get { return CameraSystem._cameraComponent.Position; }
        }

        // for now, camera follows entity with eid <see cref="CurCameraEid"/>
        public static void Update(EntityComponentStorage ecs, GameTime gameTime)
        {
            if (CurCameraEid != null)
            {
                _cameraComponent = (CameraComponent)(ecs.Entities[(int)CurCameraEid].
                                        Components[typeof(CameraComponent)]);

                PositionComponent posComp = (PositionComponent)(ecs.Entities[(int)CurCameraEid].
                                        Components[typeof(PositionComponent)]);
                MovementComponent moveComp = (MovementComponent)(ecs.Entities[(int)CurCameraEid].
                                        Components[typeof(MovementComponent)]);

                if (_cameraComponent != null && posComp != null)
                {
                    var moveFactor = moveComp.MoveSpeed *
                                                    (float)gameTime.ElapsedGameTime.TotalSeconds;

                    _cameraComponent.Position +=
                    new Vector3((posComp.Position.X - _cameraComponent.Position.X) *
                                                                                    moveFactor.X,
                                (posComp.Position.Y - _cameraComponent.Position.Y) *
                                                                                    moveFactor.Y,
                                0
                                );
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Sorry Sir! Null component reference " +
                                                        "in camera system.");
                }
            }
        }
    }
}
