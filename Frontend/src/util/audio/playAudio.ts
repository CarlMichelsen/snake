export const playAudio = async (audioPath: string) => {
    const context = new AudioContext();
    const response = await fetch(audioPath);
    const buffer = await response.arrayBuffer();
    const audioBuffer = await context.decodeAudioData(buffer);

    const source = context.createBufferSource();
    source.buffer = audioBuffer;
    source.connect(context.destination);
    source.start();
}