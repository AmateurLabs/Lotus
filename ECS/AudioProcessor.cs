using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL;

namespace Lotus.ECS {

    public sealed class AudioProcessor : Processor, IDisposable {

        AudioContext ctx;
        public Stack<int> SourcePool;

        public static Dictionary<AudioClip, int> Buffers = new Dictionary<AudioClip, int>();
        public Dictionary<Tuple<int, int>, int> Sources = new Dictionary<Tuple<int, int>, int>();

        /// <summary>The maximum number of underlying OpenAL sources to use. Hardware usually caps around 32.</summary>
        const int MAX_SOURCES = 32;

        public AudioProcessor() {
            ctx = new AudioContext();
            if (ctx.CurrentDevice != "OpenAL Soft") Console.WriteLine("Using hardware drivers. Sound may not work correctly.");
            SourcePool = new Stack<int>(AL.GenSources(MAX_SOURCES));

            AL.DopplerFactor(0f); //Disable doppler effect, helps prevent crackling

            Window.Main.Closed += (sender, e) => {
                Dispose();
            };
        }

        /// <summary>Loads the correct internal OpenAL enum given the channels (stereo or mono) and the bit depth (8 or 16).</summary>
        public static ALFormat GetSoundFormat(int channels, int bits) {
            if (channels == 1 && bits == 8) return ALFormat.Mono8;
            if (channels == 1 && bits == 16) return ALFormat.Mono16;
            if (channels == 2 && bits == 8) return ALFormat.Stereo8;
            if (channels == 2 && bits == 16) return ALFormat.Stereo16;
            throw new NotSupportedException("The specified sound format (" + channels + " channels, " + bits + " bits) is not supported.");
        }

        public override void Render() {
            
            Debug.DrawUILater(() => {
                /*foreach (AudioSource src in Entity.GetAll<AudioSource>()) {
                    Debug.DrawText(0f, 12f, "Time: " + src.Time.Value + " State: " + src.State.Value);
                    AudioClip clip = src.Clip.Value;
                    short[] data = clip.GetData();
                    GL.Begin(PrimitiveType.LineLoop);
                    GL.Color4(1f, 0f, 0f, 1f);
                    GL.Vertex3(0.0, 512.0, 0.0);
                    for (int i = 0; i < data.Length; i++) {
                        GL.Vertex3((0.0 + i) / data.Length * 512.0, 512.0 - ((double)data[i]) / short.MaxValue * 256.0, 0.0);
                    }
                    GL.Vertex3(512.0, 512.0, 0.0);
                    GL.End();
                }*/
            });
        }

        public override void Update(float dt) {
            foreach (AudioListener al in Entity.GetAll<AudioListener>()) {
                foreach (AudioSource src in Entity.GetAll<AudioSource>()) {
                    var pair = new Tuple<int, int>(al.Id, src.Id);
                    if ((al.LayerMask.Value & src.LayerMask.Value) == 0) {
                        if (Sources.ContainsKey(pair)) {
                            AL.SourceStop(Sources[pair]);
                            SourcePool.Push(Sources[pair]);
                            Sources.Remove(pair);
                        }
                        continue;
                    }
                    if (src.State.Value == AudioSourceState.Playing) {
                        Vector3 pos = Vector3.Zero;
                        if (Entity.Has<Transform>(src.Id)) {
                            pos = Entity.Get<Transform>(src.Id).Position.Value;
                        }
                        if (Entity.Has<Transform>(al.Id)) {
                            pos = Entity.Get<Transform>(al.Id).ToObjectPoint(pos);
                            pos.X = -pos.X;
                        }
                        AL.Source(Sources[pair], ALSource3f.Position, ref pos);
                        AL.Source(Sources[pair], ALSourcef.Pitch, src.Pitch.Value);
                        AL.Source(Sources[pair], ALSourceb.Looping, src.Looping.Value);

                        float time;
                        AL.GetSource(Sources[pair], ALSourcef.SecOffset, out time);
                        src.Time.Value = time;

                        if (AL.GetSourceState(Sources[pair]) == ALSourceState.Stopped)
                            src.State.Value = AudioSourceState.Stop;
                    }
                    if (src.State.Value == AudioSourceState.Play) {
                        if (!Sources.ContainsKey(pair) && SourcePool.Count > 0) Sources[pair] = SourcePool.Pop();
                        if (Sources.ContainsKey(pair)) {
                            if (!Buffers.ContainsKey(src.Clip.Value)) {
                                int bufId = AL.GenBuffer();
                                Buffers.Add(src.Clip.Value, bufId);
                                short[] soundData = src.Clip.Value.GetData();
                                AL.BufferData(bufId, GetSoundFormat((src.Clip.Value.Stereo) ? 2 : 1, 16), soundData, soundData.Length * 2, src.Clip.Value.SampleRate);
                                AL.Source(Sources[pair], ALSourcei.Buffer, Buffers[src.Clip.Value]);
                            }
                            AL.Source(Sources[pair], ALSourcef.Pitch, src.Pitch.Value);
                            AL.Source(Sources[pair], ALSourceb.Looping, src.Looping.Value);
                            AL.SourcePlay(Sources[pair]);
                            src.State.Value = AudioSourceState.Playing;
                        }
                    }
                    if (src.State.Value == AudioSourceState.Stop) {
                        AL.SourceStop(Sources[pair]);
                        SourcePool.Push(Sources[pair]);
                        Sources.Remove(pair);
                        src.State.Value = AudioSourceState.Stopped;
                        src.Time.Value = 0f;
                    }
                    if (src.State.Value == AudioSourceState.Pause) {
                        AL.SourcePause(Sources[pair]);
                        src.State.Value = AudioSourceState.Paused;
                    }
                }
            }
            Debug.AddMsg("Audio Buffers: " + Buffers.Count);
            Debug.AddMsg("Audio Sources: " + Sources.Count + " (" + SourcePool.Count + " free)");
        }

        public override void Reveille(Component component) {

        }

        public override void Taps(Component component) {
            if (component is AudioListener) {
                AudioListener al = component as AudioListener;
                foreach (AudioSource src in Entity.GetAll<AudioSource>()) {
                    var pair = new Tuple<int, int>(al.Id, src.Id);
                    if (Sources.ContainsKey(pair)) {
                        AL.SourceStop(Sources[pair]);
                        SourcePool.Push(Sources[pair]);
                        Sources.Remove(pair);
                    }
                }
            }
            if (component is AudioSource) {
                AudioSource src = component as AudioSource;
                foreach (AudioListener al in Entity.GetAll<AudioListener>()) {
                    var pair = new Tuple<int, int>(al.Id, src.Id);
                    if (Sources.ContainsKey(pair)) {
                        AL.SourceStop(Sources[pair]);
                        SourcePool.Push(Sources[pair]);
                        Sources.Remove(pair);
                    }
                }
            }
        }

        public void Dispose() {
            foreach (int srcId in Sources.Values) AL.DeleteSource(srcId);
            foreach (int bufId in Buffers.Values) AL.DeleteBuffer(bufId);
            ctx.Dispose();
            ctx = null;
        }
    }
}
