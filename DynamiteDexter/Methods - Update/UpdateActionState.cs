using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public void UpdateActionState()
        {
            bool swapLayout = false;
            Room currentRoom = WorldCursorInRange() ? worldSet[worldCursor.X, worldCursor.Y] : null;

            foreach (IGameObject i in spriteSet)
            {
                i.Update();

                if (i is IFires && (i as IFires).Chamber != null)
                {
                    IFires iFires = i as IFires;

                    foreach (IProjectile j in iFires.Chamber)
                    {
                        if (j != null)
                            pendingSet.Add(j as IGameObject);

                        if (j is GravityArc)
                            pendingSet.Add((j as GravityArc).Subject);

                        if (j is IOutlined)
                            pendingSet.Add((j as IOutlined).TheOutlineMask);
                    }

                    iFires.EmptyChamber();
                }

                if (i is ISeeks && universalTimer % (i as ISeeks).PathUpdateInterval == 0)
                {
                    ISeeks iSeeks = i as ISeeks;
                    
                    if (iSeeks.DynamicGraph)
                    {
                        if (i is Snake)
                            iSeeks.AcquireGraph(GenerateGraph(spriteSet, (i as Snake).Segment[0]));
                        else iSeeks.AcquireGraph(GenerateGraph(spriteSet));
                    }

                    iSeeks.UpdatePath();
                }

                if (i is INavigates && universalTimer % 90 == 0)
                    (i as INavigates).AcquireSet(spriteSet);

                //Collision Checks

                if (i is Pedestrian)
                {
                    bool crabInWater = false;

                    foreach (IGameObject j in spriteSet)
                    {
                        Pedestrian iPedestrian = i as Pedestrian;
                        Rectangle iHitBox = iPedestrian.HitBoxTerrain;

                        if (j is Terrain && iHitBox.Intersects(j.Rect))
                        {
                            bool terrainException = j is Passable || (j is Gate && !(j as Gate).Closed);

                            if (!terrainException)
                            { 
                                if (i is Crab && j is Water)
                                {
                                    (i as Crab).Submerged = true;
                                    crabInWater = true;
                                }
                                else if (i is IDestructive && j is Breakable)
                                {
                                    if (j is Barrel)
                                    { 
                                        pendingSet.Add(new Explosion(j));
                                        lightUpTimer = LIGHT_UP_TIME;
                                    }
                                    else pendingSet.Add(new Crumble(j));

                                    removalSet.Add(j);
                                }
                                else if (i is Player && j is Door && (i as Player).Keys > 0)
                                {
                                    Door jDoor = j as Door;
                                    removalSet.Add(j);
                                    if (jDoor.OtherHalf != null)
                                        removalSet.Add(jDoor.OtherHalf);
                                    (i as Player).DecreaseKeys();
                                    PlaySound(Sounds.OPEN_DOOR);
                                }
                                else //Clean up code for nudging later.
                                {
                                    const int NUDGE_MARGIN = 4;
                                    bool nudgedVertically = false;
                                    iPedestrian.ContactWithTerrain = true;

                                    if (iHitBox.Left <= j.Right && i.X < iPedestrian.PreviousX)
                                    {
                                        if (i is Player)
                                        {
                                            if (iHitBox.Top >= j.Bottom + 1 - NUDGE_MARGIN)
                                            {
                                                Rectangle adjacentBelow = new Rectangle(j.Left, j.Bottom, 1, 1);
                                                bool adjacencyFound = false;

                                                foreach (IGameObject k in spriteSet)

                                                    if (k is Terrain && k.Rect.Intersects(adjacentBelow))
                                                    {
                                                        adjacencyFound = true;
                                                        break;
                                                    }
                                                
                                                if (!adjacencyFound)
                                                {
                                                    iPedestrian.SetHitBoxTop(0, j.Bottom + 1, absolute: true);
                                                    nudgedVertically = true;
                                                }
                                            }
                                            else if (iHitBox.Bottom < j.Top + NUDGE_MARGIN)
                                            {
                                                Rectangle adjacentAbove = new Rectangle(j.Left, j.Top - 1, 1, 1);
                                                bool adjacencyFound = false;

                                                foreach (IGameObject k in spriteSet)

                                                    if (k is Terrain && k.Rect.Intersects(adjacentAbove))
                                                    {
                                                        adjacencyFound = true;
                                                        break;
                                                    }

                                                if (!adjacencyFound)
                                                {
                                                    iPedestrian.SetHitBoxBottom(0, j.Top, absolute: true);
                                                    nudgedVertically = true;
                                                }
                                            }
                                            else iPedestrian.SetHitBoxLeft(0, j.Right + 1, absolute: true);
                                        }
                                        else iPedestrian.SetHitBoxLeft(0, j.Right + 1, absolute: true);
                                    }
                                    else if (iHitBox.Right > j.Left && i.X > iPedestrian.PreviousX)
                                    {
                                        if (i is Player)
                                        {
                                            if (iHitBox.Top >= j.Bottom + 1 - NUDGE_MARGIN)
                                            {
                                                Rectangle adjacentBelow = new Rectangle(j.Left, j.Bottom, 1, 1);
                                                bool adjacencyFound = false;

                                                foreach (IGameObject k in spriteSet)

                                                    if (k is Terrain && k.Rect.Intersects(adjacentBelow))
                                                    {
                                                        adjacencyFound = true;
                                                        break;
                                                    }

                                                if (!adjacencyFound)
                                                {
                                                    iPedestrian.SetHitBoxTop(0, j.Bottom + 1, absolute: true);
                                                    nudgedVertically = true;
                                                }
                                            }
                                            else if (iHitBox.Bottom < j.Top + NUDGE_MARGIN)
                                            {
                                                Rectangle adjacentAbove = new Rectangle(j.Left, j.Top - 1, 1, 1);
                                                bool adjacencyFound = false;

                                                foreach (IGameObject k in spriteSet)

                                                    if (k is Terrain && k.Rect.Intersects(adjacentAbove))
                                                    {
                                                        adjacencyFound = true;
                                                        break;
                                                    }

                                                if (!adjacencyFound)
                                                {
                                                    iPedestrian.SetHitBoxBottom(0, j.Top, absolute: true);
                                                    nudgedVertically = true;
                                                }
                                            }
                                            else iPedestrian.SetHitBoxRight(0, j.Left, absolute: true);
                                        }
                                        else iPedestrian.SetHitBoxRight(0, j.Left, absolute: true);
                                    }

                                    if (iHitBox.Top <= j.Bottom && i.Y < iPedestrian.PreviousY && !nudgedVertically)
                                        iPedestrian.SetHitBoxTop(0, j.Bottom + 1, absolute: true);
                                    else if (iHitBox.Bottom > j.Top && i.Y > iPedestrian.PreviousY && !nudgedVertically)
                                        iPedestrian.SetHitBoxBottom(0, j.Top, absolute: true);
                                }
                            }
                        }
                        else if (i is Crab && !crabInWater)
                            (i as Crab).Submerged = false;
                    }
                }

                if (i is Pickup && i.Rect.Intersects(player.Rect))
                {
                    bool dollarsGoalReached = false;

                    if (i is Treasure)
                        dollarsGoalReached = player.AdjustDollars((i as Treasure).Value);
                    else if (i is DynamiteStock)
                        player.AdjustDynamiteSticks((i as DynamiteStock).Value);
                    else if (i is Key)
                        player.IncreaseKeys();
                    else if (i is ExtraLife)
                    { 
                        player.IncreaseLives();
                        screenFlashTimer = SCREEN_FLASH_TIME;
                    }

                    PlaySound((i as Pickup).Sound);

                    if (dollarsGoalReached)
                    {
                        player.IncreaseLives();
                        queuedSound = Sounds.EXTRA_LIFE;
                    }

                    removalSet.Add(i);
                }

                if (i is Player)
                {
                    DynamiteIgnited placementDynamite = (i as Player).GetDynamiteStick();

                    if (placementDynamite != null)
                    {
                        bool willPlace = true;

                        foreach (IGameObject j in spriteSet)
                        {
                            bool terrainException = j is Passable || (j is Gate && !(j as Gate).Closed);

                            if (j is Terrain && !terrainException && placementDynamite.PlacementCollision.Intersects(j.Rect) ||
                                placementDynamite.Center.X < 0 ||
                                placementDynamite.Center.X >= playfield.X ||
                                placementDynamite.Center.Y < 0 ||
                                placementDynamite.Center.Y >= playfield.Y)
                            {
                                willPlace = false;
                                break;
                            }
                        }

                        if (willPlace)
                        {
                            pendingSet.Add(placementDynamite);
                            (i as Player).AdjustDynamiteSticks(-1);
                        }
                        else PlaySound(Sounds.DENY);
                    }

                    foreach (IGameObject j in spriteSet)
                    {
                        if (j is IHostile || j is IProjectile || j is Spikes || j is Explosion || j is Laser || j is Alligator || j is SnakeSegment)
                        {
                            bool isExceptional = j is GravityArc || j is LiveFlame || j is Imp || j is Laser || j is Alligator || j is IBoss;
                            bool isMiscType = j is IHostile || j is IProjectile;

                            bool struckByBoss = j is IBoss && (j as IBoss).HitBoxAssault.Intersects(player.HitBoxAssault);
                            bool struckByLargeSnake = j is SnakeSegment && (j as SnakeSegment).GetHitBox(0).Intersects(player.HitBoxAssault);
                            bool struckBySmallSnake = j is SmallSnake && (j as SmallSnake).HitBoxAssault.Intersects(player.HitBoxAssault);
                            bool struckByImp = j is Imp && (j as Imp).Lethal && (j as Imp).GetHitBox(0).Intersects(player.HitBoxAssault);
                            bool struckByPedestrian = j is Pedestrian && (j as Pedestrian).HitBoxAssault.Intersects(player.HitBoxAssault);
                            bool struckByMiscellaneous = isMiscType && !isExceptional && j is IGameObjectPlus && (j as IGameObjectPlus).GetHitBox(0).Intersects(player.HitBoxAssault);
                            bool struckBySpikes = j is Spikes && j.Rect.Intersects(player.HitBoxAssault);
                            bool struckByExplosion = j is Explosion && (j as Explosion).StrikePlayer && IntersectsCircular(j, player);
                            bool struckByLaser = j is Laser && (j as Laser).Potent && (j as Laser).GetHitBox(0).Intersects(player.HitBoxAssault);
                            bool struckByAlligator = j is Alligator && (j as Alligator).Lethal && (j as Alligator).GetHitBox(0).Intersects(player.HitBoxAssault);
                            bool struckByImpaler = j is Impaler && (j as Impaler).HitBoxAssault.Intersects(player.HitBoxAssault);
                            bool struckByLightning = j is Cloud && !(j as Cloud).Lightning.Render;

                            if (struckByMiscellaneous && struckByLightning)
                                struckByMiscellaneous = false;

                            if (struckByBoss || struckByLargeSnake || struckBySmallSnake || struckByImp || struckByPedestrian || struckByAlligator ||
                                struckByMiscellaneous || struckBySpikes || struckByExplosion || struckByLaser || struckByImpaler)
                            {
                                if (j is Raccoon)
                                {
                                    Raccoon jRaccoon = j as Raccoon;

                                    if (!jRaccoon.HandsFull)
                                    {
                                        int amount =
                                            player.Dollars >= Raccoon.DOLLARS_TO_STEAL ?
                                            Raccoon.DOLLARS_TO_STEAL :
                                            player.Dollars;

                                        player.AdjustDollars(-amount);
                                        jRaccoon.FillHands(amount);
                                    }
                                }
                                else
                                {
                                    foreach (Gate k in gates)
                                        k.HardReset();

                                    player.DecreaseLives();
                                    causeOfDeath = j;
                                    universalTimeStamp = universalTimer;
                                    PlaySound(Sounds.STRUCK);
                                    MediaPlayer.IsRepeating = false;
                                    MediaPlayer.Stop();
                                    gameMode = GameModes.Death;
                                }
                            }
                        }
                    }
                }
                else if (i is GravityArc && i.Remove)
                {
                    GravityArc iGravityArc = i as GravityArc;

                    IGameObject impact = iGravityArc.GetImpact();
                    if (impact != null)
                        pendingSet.Add(impact);
                    removalSet.Add(iGravityArc.Subject);
                    removalSet.Add(i);
                }
                else if (i is DynamiteIgnited)
                {
                    if ((i as DynamiteIgnited).TimeUp)
                    {
                        removalSet.Add(i);
                        pendingSet.Add(new Explosion(i));
                        lightUpTimer = LIGHT_UP_TIME;
                    }
                }
                else if (i is IProjectile)
                {
                    IProjectile iProjectile = i as IProjectile;

                    if (i.Right < 0 || i.Left >= playfield.X || i.Top < 0 || i.Bottom >= playfield.Y ||
                        iProjectile.ContactWithWall)
                    {
                        removalSet.Add(i);
                        if (i is IOutlined)
                            removalSet.Add((i as IOutlined).TheOutlineMask);

                        if (i is Rocket)
                        {
                            pendingSet.Add(new Explosion(i));
                            lightUpTimer = LIGHT_UP_TIME;
                        }
                    }

                    if (!(iProjectile is GravityArc) &&
                        iProjectile.Parent != null &&
                        i is IGameObjectPlus)
                    {
                        foreach (IGameObject j in spriteSet)

                            if (j is Terrain && 
                                !(j is Water) &&
                                iProjectile.Parent != j &&
                                (i as IGameObjectPlus).GetHitBox(0).Intersects(j.Rect))
                            {
                                iProjectile.ContactWithWall = true;
                                break;
                            }
                    }
                }
                else if (i is Fireworks && (i as Fireworks).Release != null)
                {
                    pendingSet.Add(new Explosion((i as Fireworks).Release));
                }
                else if (i is Explosion && (i as Explosion).Strike)
                {
                    foreach (IGameObject j in spriteSet)
                    {
                        if (j is Breakable && IntersectsCircular(i, j))
                        {
                            if (j is Barrel)
                            {
                                pendingSet.Add(new Explosion(j));
                                pendingSet.Add(new Fireworks(j, duration: 20));
                            }
                            else if (j is Drum)
                            {
                                pendingSet.Add(new Explosion(j));
                                pendingSet.Add(new Fireworks(j, duration: 60));
                            }
                            else if (j is Orb)
                            {
                                pendingSet.Add(new Crumble(j));
                                swapLayout = true;
                            }
                            else if (j is Terminal)
                            {
                                pendingSet.Add(new Crumble(j));
                                (j as Terminal).Destroy();
                            }
                            else if (j is Casket)
                            {
                                pendingSet.Add(new Crumble(j));

                                int result = rand.Next(10);

                                if (result < 2)
                                    pendingSet.Add(new Diamond((j as Casket).GridX, (j as Casket).GridY));
                                else if (result < 6)
                                    pendingSet.Add(new MoneyBag((j as Casket).GridX, (j as Casket).GridY));
                                else if (result < 8)
                                    pendingSet.Add(new Coins((j as Casket).GridX, (j as Casket).GridY));
                            }
                            else if (j is BreakablePrize)
                            {
                                BreakablePrize jBreakablePrize = j as BreakablePrize;
                                int result = rand.Next(16);

                                if (result < 1)
                                    pendingSet.Add(new MoneyBag(jBreakablePrize.GridX, jBreakablePrize.GridY));
                                else if (result < 4)
                                    pendingSet.Add(new Coins(jBreakablePrize.GridX, jBreakablePrize.GridY));

                                removalSet.Add(j);
                                pendingSet.Add(new Crumble(j));
                            }
                            else pendingSet.Add(new Crumble(j));

                            removalSet.Add(j);
                        }
                        else if (j is GhostCasket && IntersectsCircular(i, j))
                        {
                            pendingSet.Add(new Diamond((j as GhostCasket).GridX, (j as GhostCasket).GridY));
                            pendingSet.Add(new Crumble(j));
                            removalSet.Add(j);
                        }
                        else if (j is SnakeTail && IntersectsCircular(i.Rect, (j as SnakeTail).GetHitBox(0, absolute: true)))
                        {
                            SnakeTail jSnakeTail = j as SnakeTail;

                            jSnakeTail.Parent.Strike();

                            if (jSnakeTail.Parent.Remove)
                            {
                                pendingSet.Add(new Debris(j));
                                pendingSet.Add(new Debris(jSnakeTail.Parent));
                                foreach (SnakeSegment k in jSnakeTail.Parent.Segment)
                                    pendingSet.Add(new Debris(k));
                                removalSet.Add(jSnakeTail.Parent);
                                removalSet.AddRange(jSnakeTail.Parent.Attachments);
                                BossRoom.IncreaseNumDefeated();
                                BossRoom.MarkAsDefeated(worldCursor.X, worldCursor.Y);
                                BossRoom.PlayDefeatAudio();
                            }
                        }
                        else if (j is IBoss && !(j as IBoss).Flashing && IntersectsCircular(i.Rect, (j as IBoss).HitBoxAssault))
                        {
                            IBoss jBoss = j as IBoss;

                            jBoss.Strike();

                            if (jBoss.Remove)
                            {
                                pendingSet.Add(new Debris(j));
                                removalSet.Add(j);
                                if (j is IHasAttachments)
                                    removalSet.AddRange((j as IHasAttachments).Attachments);
                                BossRoom.IncreaseNumDefeated();
                                BossRoom.MarkAsDefeated(worldCursor.X, worldCursor.Y);
                                BossRoom.PlayDefeatAudio();

                                if (jBoss is Dragon)
                                {
                                    winCount = 60;
                                    winBig = BossRoom.AllDefeated;

                                    foreach (IGameObject k in spriteSet)
                                        if (k is IProjectile || k is Blade)
                                            removalSet.Add(k);
                                }
                            }
                        }
                        else if (j is IHostile && (j is Pedestrian || j is Hypnoball || j is SmallSnake) && !(j is IBoss) && IntersectsCircular(i, j))
                        {
                            pendingSet.Add(new Debris(j));
                            removalSet.Add(j);

                            if (j is IHasAttachments)
                                removalSet.AddRange((j as IHasAttachments).Attachments);

                            if (j is Raccoon && (j as Raccoon).HandsFull)
                                pendingSet.Add(new LootBag(j.X / TILE_SIZE, j.Y / TILE_SIZE, (j as Raccoon).DollarsStolen));
                            else if (rand.Next(2) == 0)
                            {
                                if (rand.Next(4) == 0)
                                    pendingSet.Add(new MoneyBag(j.X / TILE_SIZE, j.Y / TILE_SIZE));
                                else pendingSet.Add(new Coins(j.X / TILE_SIZE, j.Y / TILE_SIZE));
                            }
                        }
                    }

                    PlaySound(Sounds.EXPLOSION);
                }
                else if (i is Flag)
                {
                    Flag iFlag = i as Flag;
                    Rectangle playerHitBox = player.HitBoxAssault;

                    if (iFlag.Rect.Intersects(playerHitBox) || iFlag.PoleBase.Rect.Intersects(playerHitBox))
                    {
                        if (!flagStatus[iFlag.Number].raised)
                            PlaySound(Sounds.EXTRA_LIFE);//
                        flagStatus[iFlag.Number].raised = true;
                        flagStatus[iFlag.Number].mapLocation = new Point(worldCursor.X, worldCursor.Y);
                        flagStatus[iFlag.Number].startLocation = new Point(iFlag.X + TILE_SIZE, iFlag.Y + TILE_SIZE);
                    }
                }
                else if (i is Plaque)
                {
                    if (i.Rect.Intersects(player.Rect))
                    {
                        gameMode = GameModes.Text;
                        universalTimeStamp = universalTimer;
                        textMessage.ChangeText(currentRoom.PlaqueText);
                        textPlaque.ChangeText(currentRoom.PlaqueText);
                        player.Reposition(i.Left, i.Top + 2 * TILE_SIZE);
                        player.Neutral();
                    }
                }
                else if (i is WorldMap)
                {
                    if (i.Rect.Intersects(player.Rect))
                    {
                        illustration = Images.BACKGROUND_MAP;
                        gameMode = GameModes.Illustration;
                        universalTimeStamp = universalTimer;
                        player.Reposition(i.Left, i.Top + 2 * TILE_SIZE);
                        player.Neutral();
                    }
                }
                else if (i is Trippable)
                {
                    Trippable iTrippable = i as Trippable;

                    if (!iTrippable.Tripped && iTrippable.TripZone.Intersects(player.Rect))
                    {
                        pendingSet.AddRange(iTrippable.GetItem(player));
                        iTrippable.Trip();
                    }
                }
                else if (i is Angel && i.X < -i.Width)
                {
                    removalSet.Add(i);
                    removalSet.AddRange((i as IHasAttachments).Attachments);
                }
                else if (i is SuperComputer)
                {
                    Laser[] components = (i as SuperComputer).ComponentRemoval();

                    if (components != null)
                        foreach (Laser j in components)
                            removalSet.Add(j);
                }
                else if (i is Tank)

                    (i as Tank).RepositionBarrel();

                if (i is IOutlined)
                    (i as IOutlined).SyncOutline();

                if (i is IExpires && (i as IExpires).TimeUp)
                    removalSet.Add(i);
            }

            foreach (IGameObject i in removalSet)
            {
                spriteSet.Remove(i);

                if (i is IOutlined)
                    spriteSet.Remove((i as IOutlined).TheOutlineMask);
            }
            removalSet.Clear();

            foreach (IGameObject i in pendingSet)
                spriteSet.Add(i);
            pendingSet.Clear();

            if (swapLayout)
            {
                currentRoom.SwapLayout(alteredSet[worldCursor.X, worldCursor.Y]);
                spriteSet = currentRoom.SpriteSet;
                queuedSound = Sounds.ALTERED_PATH;
            }

            if (currentRoom != null && currentRoom.TheMode == Room.Mode.Boss)
            {
                if (!BossRoom.Defeated(worldCursor) && BossRoom.InPreBattle)
                { 
                    BossRoom.ProcessPreBattle(worldCursor, player);

                    if (BossRoom.BossAppears)
                    {
                        BossRoom.LoadBlockade(spriteSet);
                        spriteSet.Add(BossRoom.TheBoss.AppearingForm);
                        spriteSet.AddRange(BossRoom.Blockade);
                    }
                    else if (BossRoom.BattleBegins)
                    {
                        if (BossRoom.TheBoss is IGameObject)
                            spriteSet.Add(BossRoom.TheBoss as IGameObject);
                        else throw new System.Exception("Error - Boss must be a game object.");

                        if (BossRoom.TheBoss is IHasAttachments)
                            spriteSet.AddRange((BossRoom.TheBoss as IHasAttachments).Attachments);

                        spriteSet.Remove(BossRoom.TheBoss.AppearingForm);
                    }
                }
                else if (BossRoom.InBattle(worldCursor))
                {
                    if (BossRoom.CheckDynamiteSupply(player))
                        spriteSet.AddRange(BossRoom.GetSympathyStick(worldCursor));
                }
                else if (BossRoom.Defeated(worldCursor) && BossRoom.Blockade.Count > 0)
                
                    foreach (Terrain i in BossRoom.Blockade)
                        spriteSet.Remove(i);
            }

            //Bounds check.

            bool edgeReached = false;
            Point previousLocation = worldCursor;

            if (player.Center.X >= playfield.X)
            {
                player.Reposition(-(int)(player.Width * 0.25f), player.Y);
                worldCursor = new Point(worldCursor.X + 1, worldCursor.Y);
                edgeReached = true;
            }
            else if (player.Center.X < 0)
            {
                player.Reposition(playfield.X - (int)(player.Width * 0.75f), player.Y);
                worldCursor = new Point(worldCursor.X - 1, worldCursor.Y);
                edgeReached = true;
            }
            else if (player.Center.Y >= playfield.Y)
            {
                player.Reposition(player.X, -(int)(player.Height * 0.25f));
                worldCursor = new Point(worldCursor.X, worldCursor.Y + 1);
                edgeReached = true;
            }
            else if (player.Center.Y < 0)
            {
                player.Reposition(player.X, playfield.Y - (int)(player.Height * 0.75f));
                worldCursor = new Point(worldCursor.X, worldCursor.Y - 1);
                edgeReached = true;
            }

            if (edgeReached)
                ChangeRoom(previousLocation.X, previousLocation.Y);

            if (screenFlashTimer > 0)
                screenFlashTimer--;

            if (lightUpTimer > 0)
                lightUpTimer--;

            if (winCount != null && --winCount == 0)
            {
                if (winBig)
                    textMessage.ChangeText(Messages.VICTORY_B);
                else textMessage.ChangeText(Messages.VICTORY_A);

                universalTimeStamp = universalTimer;
                PlayMusic(Sounds.Music.DEPARTURE);
                gameMode = GameModes.Text;
            }

            ProcessQueuedSound();

            inGameTimer++;
            universalTimer++;
        }
    }
}
