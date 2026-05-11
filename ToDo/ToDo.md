## 1. Завершить систему окон меню

Сейчас:

есть enum;
интерфейс;
manager;
одно окно.

Доделаем:

остальные окна;
регистрацию окон;
базовую навигацию.
## 2. Создать базовый MenuController

Это будет:

точка входа меню;
объект который управляет:
кнопками,
открытием окон,
стартом игры.
## 3. Создать Settings систему

Очень важный слой.

Будет:

музыка;
SFX;
ambient;
язык.

Там появятся:

SettingsData;
SettingsManager;
значения по умолчанию.
## 4. Создать Save систему

Для:

autosave;
manual saves;
загрузки;
сохранения настроек.
## 5. Создать NewGameConfig

Настройки старта игры:

difficulty
enemy multiplier
starting units
wave speed
## 6. Создать Gameplay foundation

Появятся:

GameSession;
GameState;
game loop.
## 7. Создать Wave System

Начнётся:

спавн врагов;
таймер волн;
сложность.
## 8. Создать Bonus System

Твои:

+10
x2
-10
divide
speed
buffs
## 9. Создать Army System

Игрок:

увеличивает армию;
теряет юнитов;
получает modifiers.
## 10. Только потом подключать Unity UI

И уже:

Canvas;
Buttons;
Scene;
MonoBehaviour;
prefabs.

То есть Unity станет:

визуальной оболочкой над уже готовой логикой.