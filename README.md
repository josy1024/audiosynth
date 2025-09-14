# audiosynth

* audiosynth is a simple keyboard pressed audio tone generator
* play and tryout different sounds - playground/sandboxing
* it uses simple software synthisizer technics creating audio with the Naudio Library

* transparency and open - i want to share progess

* [CHANGELOG](CHANGELOG.md)

![FRONTEND: Audio Synth Player](doc/audioSynthPlayer.png)

## why

* sound playground for me and the kids
* i like coding :-) improve make c# .net8 coding skills
* test code generation, test github workflows

## features

* wave forms (sine,Sine,Saw,Square,Triangle,FM)
* keyboard input keydown + keyup (show last 10 notes)
* main tones CDEFGA(H|B)
* half tones with strg/alt (CDFGA) => 12 buttons
* octave switching with controlled by previous track and next track
* UX - simpler user input use with key / waveform switching (by pressing x)
* visualizer

## planned

* keyboard tone mapping config to json file (key, desc, frequency)
* maybe? change tone not on button more on wheel
* want: easyer select and change instruments/VoideProvider
* json Configs for ADSR
* focus vs Field of View
* LFO Filter
* track/loop function (loop templates) ? - maybe not: lmms?
* multi-language config (german+english)
* kind of recorder?
* new instruments?

## Input Device

* Computer-Keyboard Inputs
![Mini Keyboard Example png](doc/mini-keyboard-example.png)

## CODE Core Components

* [Project CSPROJ](audiosynthSOL/audiosynth/audiosynth.csproj)
* [ADSR: Attack, Decay, Sustain, Release](audiosynthSOL/audiosynth/ADSR.cs)
* [VoideProvider Fuctions: Saw, Square, Triangle, Sine, Noise, FM](audiosynthSOL/audiosynth/VoiceProvider.cs)


