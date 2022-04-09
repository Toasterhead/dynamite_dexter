using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void UpdateWinBState()
        {
            int elapsedTime = universalTimer - universalTimeStamp;

            if (EndGameB.TheFade.Render)
                EndGameB.TheFade.Update();

            if (EndGameB.Boarding)
            {
                EndGameB.TheAvatarScripted.Update();
                EndGameB.TheSpaceshipScripted.Update();

                EndGameB.ProcessBoardingPhase();
            }
            else if (EndGameB.Cruising)
            {
                foreach (EndGameB.CelestialBody i in EndGameB.CelestialBodies)
                    i.Update();

                if (elapsedTime > 300 && elapsedTime % EndGameB.CHANGE_TEXT_INTERVAL == 0)
                    EndGameB.ChangeText();

                if (EndGameB.CreditsComplete)
                    EndGameB.BeginLanding();
            }
            else if (EndGameB.Landing)
            {
                EndGameB.TheSpaceshipScripted.Update();

                if (!EndGameB.CrashLanded && EndGameB.TheSpaceshipScripted.Bottom > EndGameB.VERTICAL_POSITION_LANDING)
                {
                    EndGameB.CrashLanded = true;
                    EndGameB.TheSpaceshipScripted.Render = false;
                    EndGameB.SpaceshipSmoke.Render = true;
                    EndGameB.BeginQuake();
                }
                else if (EndGameB.CrashLanded)
                {
                    bool avatarRighOfEntrance = EndGameB.TheAvatarSpacesuitScripted.Left > fullfield.X / 2 + 35;

                    EndGameB.TheAvatarSpacesuitScripted.Update();
                    EndGameB.SpaceshipSmoke.Update();
                    EndGameB.ProcessQuake();

                    if (avatarRighOfEntrance && EndGameB.TheAvatarSpacesuitScripted.Top <= EndGameB.PLANET_HORIZON - 2)
                    {
                        EndGameB.TheFade.Render = true;

                        if (EndGameB.TheFade.Complete)
                            EndGameB.ConcludeSequence();

                        if (EndGameB.SequenceComplete && EndGameB.ButtonPressed)
                            SubUpdateGameOver();
                    }
                    else if (avatarRighOfEntrance && EndGameB.TheAvatarSpacesuitScripted.Top <= EndGameB.PLANET_HORIZON)
                    {
                        EndGameB.TheAvatarSpacesuitScripted.Render = false;
                        EndGameB.TheFade.Render = true;
                    }
                    else if (avatarRighOfEntrance)
                    {
                        EndGameB.TheAvatarSpacesuitScripted.CurrentAction = EndGameB.AvatarScripted.Actions.Walking;
                        EndGameB.TheAvatarSpacesuitScripted.CurrentDirection = EndGameB.AvatarScripted.Directions.TowardHorizon;
                        EndGameB.TheFade = new EndGameB.Fade(6);
                    }
                    else if (EndGameB.TheAvatarSpacesuitScripted.Bottom > EndGameB.TheAvatarSpacesuitScripted.InitialY)
                    {
                        EndGameB.TheAvatarSpacesuitScripted.Bottom = EndGameB.TheAvatarSpacesuitScripted.InitialY;
                        EndGameB.TheAvatarSpacesuitScripted.CurrentAction = EndGameB.AvatarScripted.Actions.Walking;
                    }
                    else if (EndGameB.TheAvatarSpacesuitScripted.Left > EndGameB.TheSpaceshipScripted.Right - 10)
                        EndGameB.TheAvatarSpacesuitScripted.Render = true;
                    else if (EndGameB.TheAvatarSpacesuitScripted.Left > EndGameB.TheSpaceshipScripted.Right - 20)
                        EndGameB.TheAvatarSpacesuitScripted.CurrentAction = EndGameB.AvatarScripted.Actions.Jumping;
                }
            }

            universalTimer++;
        }
    }
}
