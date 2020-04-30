★Game rule:
Tetris guideline, with TOJ vs-rule, 1v1.
turn-based game by default, player first.

some exceptions:
no lock-out death, no falling speed, no slide move limit, only lock when hard drop key pushed.

★Default controls:
move left ------------- left arrow
move right ------------ right arrow
soft drop ------------- down arrow
hard drop ------------- space
rotate ccw ------------ Z
rotate 180 ------------ X
rotate cw ------------- C
hold ------------------ V

★Functional keys:
Press F12 to config your own controls (saved automatially).
Press F2 to restart the game (after finishing a game or before 20 pieces are locked).
Press F3 to toggle garbage buffer height display.
Press F4 to toggle grid display.
Press F5 and F6 to adjust SFX & BGM volume.

Default DAS is 8 frames under 60fps.

★Config:
In file "misamino.ini"
section "AI"
        "delay": AI think delay ( enable when turnbase=0 )
        "move" : AI move delay ( enable when turnbase=0 )
        "4w"   : Enable AI 4-wide combo strategy, but only enable when level>=6 and toj rule (combo table=1 or 2; tspin/tspin+/ren AI; garbage buffer, garbage blocking and garbage cancel on)

section "AI_P1" and "AI_P2"
        "style": The style of AI, P1 default is 0, P2 default is 2, style list:
0, human
1, T-spin+ AI
2, T-spin
3, Ren
4, non-Hold
5, Downstack
-1, Use the dll plugin specified below.

        "level": The level of AI is between 0 to 8. Default value is 4
        "PieceMul": In normal trunbase mode, setting the ratio of pieces
        "dllplugin": AI dll plugin file path for player. set style=-1 to enable this plugin

section "Rule"
        "turnbase": Turn-based game, 0 to have real-time game using the delay values above.
        "KOS_turnbase": 7 pieces a turn
        "spin180" : allow 180 degree rotate if set to 1
        "GarbageStyle" : Garbage line style. 0 means TOJ style; 1 means messy style (hole change every 2 lines); 2 means more messy style (hole change every line)
        "GarbageCancel"  : Allow garbage cancelling. Default is 1.
        "GarbageBuffer": Attack applies immediately if set to 0, like in older games / hangame.
        "GarbageBlocking": Clearing lines prevents the garbage from rising.
        "samesequence": P1 and P2 will have different piece sequence if set to 0.
        "combo_table_style": 0 means the combo table is [0,0,1,1,2], 1 is TOJ combo table, 2 means Tetris Friend E+ combo table. default is 1

section "Player"
        "das": The das setting, 8 frames default.
        "softdropdas": softdrop das setting, 3 frames default.
        "softdropdelay": The softdrop delay, 10 is default, means 10 * 0.1 = 1 frame

More distussion visit http://harddrop.com/forums/index.php?showtopic=5292
