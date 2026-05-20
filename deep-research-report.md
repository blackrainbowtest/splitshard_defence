# Combat Prototype Research Brief for Splitshard Defence

A ready-to-hand Markdown brief for your coding AI is attached here: [Download the build brief](sandbox:/mnt/data/LAST_BASTION_AI_BUILD_BRIEF.md)

## What your current repositories already establish

Of the two repositories you explicitly selected, **`splitshard_defence` is the better base repo for today’s prototype**. Its own README already describes the exact fantasy and loop you have been discussing: defend a castle, move left and right, collect bonuses, grow the army, and survive endless waves. The same repo is already on **Unity 6000.4.6f1** and its manifest already includes **Addressables 2.9.1**, **Input System 1.19.0**, **Newtonsoft Json 3.2.2**, **URP 17.4.0**, and **UGUI**, which lines up well with a mobile graybox combat slice. fileciteturn20file0L3-L3 fileciteturn21file0L3-L3 fileciteturn13file0L3-L3

Your `UNITY_GAME_LIGHT` repo looks more like a general starter/template project. It is on the older **Unity 6000.2.10f1** editor and includes **Input System 1.14.2** plus **Newtonsoft Json 3.2.2**, so it can still be useful as a reference for generic infrastructure or save/load ideas, but it is not the thematically correct starting point for this battle prototype. fileciteturn14file0L3-L3 fileciteturn15file0L3-L3

One important practical note: the current `splitshard_defence` manifest also includes packages such as **Visual Scripting** and **Multiplayer Center**. Those are not harmful by themselves, but they are not necessary for the single-player graybox you want to implement today, so I would treat them as “do not use” rather than “rip out immediately,” unless compile time or project noise becomes a problem. fileciteturn21file0L3-L3

## The plugin decision and why PrimeTween is a sound choice

Your instinct to prefer **PrimeTween** is defensible, but the precise reason matters. **DOTween’s base package is not paid-only**: its official site describes DOTween as **free and open-source**, and the current Unity Asset Store listing for DOTween shows the base asset as **FREE**, while DOTween Pro is the paid extension. So PrimeTween is not “the only free option.” citeturn15view1turn16view0

What makes **PrimeTween** especially attractive for your case is different: its official repo describes it as a **high-performance, allocation-free** tween library, it supports **sequences**, **callbacks**, **custom tweens**, **Inspector-serializable tween settings**, and even has a documented **UPM installation path** for derivative projects such as templates, libraries, and GitHub repositories. Its license also explicitly allows use in free and commercial binary products, while asking derivative projects to depend on PrimeTween through the package method rather than redistributing the library files directly. For a game you are building in a public repository and evolving step by step, that is a very clean fit. citeturn4view1turn6view1turn11view0turn5view0

PrimeTween’s **PRO** offering adds visual authoring features such as `TweenAnimation` and Inspector-heavy composition, but the free/runtime side already covers the exact things you need for today’s prototype: UI transitions, button feedback, hit flashes, punch/shake, simple death tweens, and HUD polish. That means you can adopt PrimeTween now without locking yourself into a paid authoring workflow. citeturn11view0

My recommendation is therefore straightforward: **use PrimeTween, do not install DOTween, and keep tweening limited to feedback and UI**. For actual gameplay movement—player squad follow, enemy advance, collision timing, cooldown timers—use ordinary gameplay code. PrimeTween should animate states and feedback, not become the battle simulation layer. PrimeTween’s documented UPM flow uses a scoped registry for `com.kyrylokuzyk` and shows a manifest dependency example for `com.kyrylokuzyk.primetween`. citeturn11view0

The rest of the stack should stay intentionally lean: **Addressables** for scene loads and any later content separation, **Input System** for touch/pointer handling, **Newtonsoft Json** for save files, **UGUI** for menus/HUD, **Audio Mixer** for grouped volume control, **ScriptableObjects** for authored data, and Unity’s built-in **`ObjectPool<T>`** instead of any third-party pooling asset. The Input System package is the newer flexible input layer that supports devices, touch, and gestures; ScriptableObjects are project assets intended for shared data and can reduce duplicated prefab data; `ObjectPool<T>` is built into `UnityEngine.Pool` specifically to reuse objects and reduce repeated instantiate/destroy overhead. citeturn3view1turn3view2turn9view0

## The minimum viable prototype that matches your goal

The **smallest correct prototype** is not “all combat systems,” but one very strict vertical slice: a **Start Battle** scene, a **Battle** scene, and a save/load rule that stores only the **phase-start snapshot**. That is the piece that proves your idea. The player should choose **level**, **difficulty**, **kingdom name**, **banner**, **hero gender**, and **hero appearance archetype**, then press **Start Battle**. At that moment, the game creates a `BattlePhaseStartSnapshot`, writes it to disk, and loads the Battle scene. When the user later loads, the game goes directly into Battle and reconstructs that phase from its start state, not from the middle of the fight. That design is not only cleaner; it radically simplifies save robustness because you do not need to serialize enemy positions, projectile states, in-flight tweens, or half-finished cooldowns. Addressables provides a direct `LoadSceneAsync` path for addressable scenes, and Unity explicitly notes that it wraps the normal scene-loading pipeline. citeturn7view0

For data authoring, use **ScriptableObjects** for levels, difficulties, banners, hero appearances, units, enemies, and wave definitions. Unity’s own documentation explains that ScriptableObjects are assets that are not attached to GameObjects and are especially useful as shared data containers, which is exactly what you want for balancing and configuration. citeturn3view2

For visual prototyping, you do **not** need multiple debug sprites. A single **white square sprite** is enough, because `SpriteRenderer.color` controls the rendering tint. That lets you differentiate squad units, enemies, castle, pickups, hero preview portraits, and banner placeholders entirely through color coding. Unity’s animation system can animate component properties such as material color and even custom script properties, so if you later want a tiny clip-driven idle/off state, that is available too; but for today, most state feedback can be code-driven with PrimeTween and color tint changes. citeturn13view1turn13view0turn6view2

This leads to the practical prototype rule set I recommend for day one:

```yaml
prototype_scope:
  scenes:
    - Bootstrap
    - StartBattle
    - Battle

  start_battle_fields:
    - level
    - difficulty
    - kingdom_name
    - banner
    - hero_gender
    - hero_appearance

  mandatory_buttons:
    - start_battle
    - load_last_battle_if_save_exists

  gameplay_core:
    player_squad:
      control: horizontal_pointer_follow
      squad_model: aggregated_roster
      save_model: phase_start_only
    combat:
      unit_types_unlocked:
        - swordsman
        - archer
        - shieldbearer
      enemy_types:
        - goblin
        - raider
        - archer_enemy
        - brute
        - ram
      economy:
        - enemy_kill_gold
        - recruit_buttons
      outcomes:
        - victory_after_waves
        - defeat_on_castle_zero

  prototype_visuals:
    sprite_strategy: one_white_square_tinted_in_runtime
    feedback:
      hit_flash: true
      death_fade: true
      attack_punch: true
      ui_feedback: true

  save_rules:
    allowed:
      - immediately_before_entering_battle
      - after_battle_result
    forbidden:
      - mid_wave
      - mid_combat
      - mid_animation
      - half_battle_resume
```

## Save, load, and battle-state rules that will keep the project sane

The cleanest implementation is to split battle data into three layers: a **setup draft** used by the Start Battle scene, a **phase-start snapshot** used for persistence, and a **runtime battle state** that is never serialized for resume. The snapshot should contain the chosen level and difficulty, kingdom metadata, selected hero preset, RNG seed, starting castle HP, starting gold, initial roster composition, and the wave-set identifier. That is enough to rebuild the fight from a deterministic start. It is intentionally **not** enough to reconstruct the middle of a battle, which is exactly the rule you asked for.

Unity’s `Application.persistentDataPath` is the appropriate place to store files you want to retain between runs, and Unity notes that the path remains stable across updates as long as the bundle identifier remains the same. citeturn12view0

Although `JsonUtility` can serialize objects into JSON, Unity documents important constraints: it serializes only supported fields, and the top-level JSON must be an object rather than a raw list or array. Your current repository already includes Unity’s Newtonsoft package, and Unity describes that package as a flexible JSON serializer for converting between .NET objects and JSON. For a versioned save schema with dictionaries and future migrations, **Newtonsoft Json is the safer choice**. citeturn12view1turn10view0

For scene flow, keep it very simple. Use `Bootstrap` to initialize services, then enter `StartBattle`, and on start use `Addressables.LoadSceneAsync` in **single** mode to enter the `Battle` scene. Unity’s Addressables docs explicitly warn that `activateOnLoad = false` blocks the async operation queue, so for this prototype there is no reason to be fancy—just activate immediately. citeturn7view0

What this means in practice is the following save policy:

```text
Allowed save windows:
- StartBattle scene, right before entering Battle
- Post-result screen after Victory/Defeat

Forbidden save windows:
- Any active battle loop
- Wave currently running
- Enemies alive
- Projectiles active
- Runtime combat state changing

Load behavior:
- From menu only
- Load latest BattlePhaseStartSnapshot
- Open Battle scene
- Recreate phase from beginning
```

That policy keeps your save system honest, testable, and far less fragile than “save everywhere.”

## Project structure and the assets you actually need

Your project should separate **data**, **runtime code**, **prefabs**, **scenes**, and **debug art** immediately. For this prototype, most of the “asset” work is really **authoring discipline**, not shopping for store packages.

A practical structure for this combat slice is:

```text
Assets/
  Audio/
    Mixers/
    Music/
    SFX/
  Data/
    ScriptableObjects/
      Levels/
      Difficulties/
      Banners/
      HeroAppearances/
      Units/
      Enemies/
      Waves/
  Prefabs/
    Bootstrap/
    UI/
    Battle/
    Enemies/
    Projectiles/
    Pickups/
  Scenes/
    Bootstrap.unity
    StartBattle.unity
    Battle.unity
  Scripts/
    Core/
    Save/
    StartBattle/
    Battle/
    UI/
  Sprites/
    Debug/
      WhiteSquare.png
    UI/
```

For code modules, I would keep the first pass to **Core**, **Save**, **StartBattle**, **Battle**, and **UI**, with battle sub-folders for **Units**, **Enemies**, **Combat**, **Waves**, and **Economy**. If you want assembly definitions from day one, that is reasonable here because the boundaries are obvious and small.

The asset list for day one is intentionally tiny. You need one square sprite, one or two placeholder UI panels, a placeholder coin icon, a handful of banner/portrait placeholders, and a few sounds. Everything else should be data-driven. Because `SpriteRenderer.color` handles tinting, you do not need separate blue, red, yellow, green, and gray square files. Because Animation Clips can animate component properties, and PrimeTween can handle hit punch/shake/fade, you also do not need a real sprite-sheet animation pipeline yet. citeturn13view1turn13view0turn6view2

For runtime object churn—enemies, projectiles, floating rewards, damage popups—use Unity’s built-in `ObjectPool<T>`. Unity documents that it is a stack-based reusable pool meant to reduce the overhead of frequent instantiation and destruction. That makes a third-party pooling package unnecessary for this prototype. citeturn9view0

For audio, use an **Audio Mixer** with at least `Master`, `Music`, `SFX`, and `UI` groups. Unity’s Audio Mixer is designed exactly for grouped control over audio routing and effects, which is enough for the current slice. citeturn7view2

## The brief you should hand to your coding AI

The attached Markdown file already packages the project into an actionable build brief. The most important thing in that file is not the folder tree or the balance values; it is the **execution contract**. If you want another AI to build this with you **step by step**, it must be told to work in small vertical tasks, stop after each one, and report exactly what it changed.

The key clauses I would not remove are these:

```text
- Work step by step.
- After each completed step, stop and report.
- Do not silently refactor unrelated code.
- Keep classes small and single-purpose.
- Use PrimeTween, not DOTween.
- Use Input System for pointer/touch.
- Use Addressables for scene loading.
- Use Newtonsoft.Json for save files.
- Save only at phase start or after result.
- Never save during active battle.
- Loading a battle always restarts that phase from the beginning.
- If something is unclear and it affects architecture or save format, state the assumption and stop for confirmation.
```

The report format matters just as much as the coding rules. The AI should finish each step with a structured status block such as: step completed, goal, files created, files modified, public classes added, what works now, manual test steps, known limitations, assumptions made, and the next recommended step. That is what prevents ambiguity, stealth refactors, and “I changed a lot of things because it felt cleaner” behavior.

My final recommendation, based on your repo state and today’s target, is this:

Use **`splitshard_defence` as the base**, keep the current Unity 6 editor line, add **PrimeTween via UPM**, keep **Addressables + Input System + Newtonsoft + UGUI + Audio Mixer + ScriptableObjects + built-in ObjectPool**, do the whole visual pass with **one white square sprite tinted at runtime**, and enforce a **phase-start-only save model** from the very beginning. That combination is the shortest path to a testable combat prototype without poisoning the codebase with unnecessary systems. fileciteturn20file0L3-L3 fileciteturn21file0L3-L3 fileciteturn13file0L3-L3 citeturn11view0turn3view1turn10view0turn9view0turn12view0