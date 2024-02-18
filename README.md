# tower-of-colors

Hello and thanks for your time!

I’ll explain here what my approach was on the required items for this test.


<b>1) Optimizations</b>

<b>1.1) UI Draw calls</b>

To optimize UI draw calls I first checked the game UI performance. So using Frame Debugger we are getting 15 draw calls in the first scene.
Each sprite was spending 1 draw call so a good approach to improve that is to use sprite atlases.
After setting it up and creating a UI/Atlases/GenericAtlas.spriteatlas, we reduced from 15 to 5 draw calls in the first scene.

Now we have:
- The first draw call is canvas related;
- the 2nd one draw all sprites;
- The 3rd one draw all text;
- 4th and 5th are Graphy lib related.

I believe this is a good optimization and also a scalable one. By adding more UI elements and sprites, we only need to update the atlas. So we are good to go to the next optimization.

<b>1.2) Pooling system for the barrels</b>

The game was already using a singleton class called FxPool to pool particles. My idea was to refactor it a little in order to support TowerTiles.

I also changed TowerTile a little so it now reset all internal states when returning to the pool. The “destroy” references were reworked too.

Also in Tower class it was necessary to introduce some changes:
- It must reset the tiles when it is destroyed. This way before the scene is reloaded for the next level, all tiles are returned back to the pool.
- TowerTiles are not anymore children of the Tower. This way we can apply DontDestroyOnLoad on them.
- Each tile must be removed from tilesByFloor when it is returned to the pool, otherwise the tower height would not be correctly calculated.

<b>2) Missions</b>

The UX is composed of a missions button that triggers the missions popup.

Missions implementation took some time. I implemented some ScriptableObjects to control the calibration. I separated it into easy, medium and hard difficulty curves, but after tuning it a little I think just a multiplier on a base curve would be enough, but for a better calibration a curve for each difficulty is better.

I also included default currencies as reward (coins and gems). A shop is missing, unfortunately.

<b>3) Time spent on the test</b>

| Step | Time |
| --- | --- |
| Setup  | ~40 min  |
| 1st optimization (UI draw calls)  | ~30 min  |
| 2nd optimization (pooling)  | ~2h30 min  |
| New UI: currencies + missions  | ~2h min  |
| Missions architecture  | ~4h min  |
| Currencies / Player wallet  | ~1h min  |
| Testing / fixing my own bugs  | ~1h  |
| Total  | ~11h40  |

Sorry, I had to use more than 8h.

<b>4) Some random points</b>

4.1) I extended the save system to keep using PlayerPrefs, but a good improvement would be to change it to use a file system.

4.2) There are bugs when trying to build the tower in editor mode. I didn’t try to fix it, sorry!

4.3) I downloaded HOMA Tower Color for Android while doing the test. The game is very fun and polished, but there is an annoying bug: some AppLovin ads show a black header that hides the close button. It seemed to me the only option was to kill the app, but doing that the rewarded ads does not count, so it is a double frustration for the player.
Bug occurred in an Android 10 on a Samsung Galaxy S9.

<b>5) Conclusion</b>

It was a pleasure to participate in this test. I hope what I did can live up to your expectations. Please let me know if there is something that is not clear or if there is something that I missed. Thanks for the opportunity!

