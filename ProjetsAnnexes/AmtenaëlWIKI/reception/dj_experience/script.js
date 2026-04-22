const experience = document.getElementById('experience');
const startBtn = document.getElementById('start-btn');
const introOverlay = document.getElementById('intro-overlay');
const subtitle = document.getElementById('subtitle');
const statusText = document.getElementById('status-text');
const hypeProgress = document.getElementById('hype-progress');
const strobeLayer = document.getElementById('strobe-layer');
const djAvatar = document.getElementById('dj-avatar');

const btnSync = document.getElementById('btn-sync');
const btnDrop = document.getElementById('btn-drop');
const btnAmtenael = document.getElementById('btn-amtenael');

let hypeLevel = 0;
let currentPhase = 'chill';
let audioCtx = null;
let masterGain = null;
let isAudioPlaying = false;
let beatInterval = null;

const SUBTITLES = {
    chill: ["CLIQUEZ POUR LANCER LE SET", "LA CONNEXION EST TOTALE"],
    syncing: ["RECHERCHE DE PÉRIPHÉRIQUES...", "BLUETOOTH CONNECTÉ !", "TOTALEMENT RAISON !", "EN BLUETOOTH !!", "LA POÎGNEE DE MAIN !"],
    dropping: ["PRÉPAREZ-VOUS AU DROP...", "LA BASS EST SANS FIL !", "QUALITÉ 128kbps OPTIMALE", "C'EST ÇA LA MAGIE !"],
    amtenael: ["SYNCHRONISATION DU WIKI...", "MIGRATION PAR LA VIBE", "AMTENAËL EST PRÊT !", "VIVE LE DJ BLUETOOTH !", "LOGS EXTRAITS PAR LES BASSES"]
};

function updateSubtitle(text) {
    subtitle.classList.remove('subtitle');
    void subtitle.offsetWidth; // Trigger reflow
    subtitle.innerHTML = text;
    subtitle.classList.add('subtitle');
}

function flashStrobe(duration = 200, count = 1) {
    let flashes = 0;
    const interval = setInterval(() => {
        strobeLayer.style.opacity = flashes % 2 === 0 ? '0.3' : '0';
        flashes++;
        if (flashes >= count * 2) {
            clearInterval(interval);
            strobeLayer.style.opacity = '0';
        }
    }, duration / 2);
}

function setPhase(phase) {
    currentPhase = phase;
    experience.className = `vibe-stage vibe-${phase}`;
    statusText.innerText = phase.toUpperCase();
    
    const lines = SUBTITLES[phase];
    updateSubtitle(lines[Math.floor(Math.random() * lines.length)]);
}

function createKick(time, intensity) {
    if (!audioCtx) return;
    const osc = audioCtx.createOscillator();
    const gainNode = audioCtx.createGain();
    
    // Distort based on phase
    const decay = currentPhase === 'chill' ? 0.3 : (currentPhase === 'syncing' ? 0.4 : 0.6);
    const startFreq = intensity > 50 ? 200 : 150;
    
    osc.connect(gainNode);
    gainNode.connect(masterGain);
    
    osc.frequency.setValueAtTime(startFreq, time);
    osc.frequency.exponentialRampToValueAtTime(0.01, time + decay);
    
    gainNode.gain.setValueAtTime(1, time);
    gainNode.gain.exponentialRampToValueAtTime(0.01, time + decay);
    
    osc.start(time);
    osc.stop(time + decay);
}

function startBeat() {
    if (isAudioPlaying) return;
    isAudioPlaying = true;
    
    let nextBeatTime = audioCtx.currentTime + 0.1;
    const intervalTime = 60000 / 128; // 128 BPM => ms per beat
    
    beatInterval = setInterval(() => {
        if (!audioCtx) return;
        nextBeatTime = audioCtx.currentTime + 0.1;
        createKick(nextBeatTime, hypeLevel);
    }, (intervalTime / 2)); // Play twice per beat for more energy (bassline feel)
}

startBtn.addEventListener('click', () => {
    introOverlay.style.display = 'none';
    
    // Init audio on first interaction
    if (!audioCtx) {
        audioCtx = new (window.AudioContext || window.webkitAudioContext)();
        masterGain = audioCtx.createGain();
        masterGain.connect(audioCtx.destination);
        masterGain.gain.value = 0.5;
    }
    if (audioCtx.state === 'suspended') {
        audioCtx.resume();
    }
    
    setPhase('chill');
    hypeLevel = 20;
    hypeProgress.style.width = '20%';
    
    startBeat();
});

btnSync.addEventListener('click', () => {
    setPhase('syncing');
    hypeLevel = Math.min(hypeLevel + 15, 100);
    hypeProgress.style.width = `${hypeLevel}%`;
    flashStrobe(100, 2);
});

btnDrop.addEventListener('click', () => {
    setPhase('dropping');
    hypeLevel = 100;
    hypeProgress.style.width = '100vw';
    
    if (masterGain) masterGain.gain.value = 1.2; // Boost volume
    
    experience.classList.add('vibration-active');
    flashStrobe(50, 20);
    
    setTimeout(() => {
        experience.classList.remove('vibration-active');
        hypeLevel = 50;
        hypeProgress.style.width = '50%';
        updateSubtitle("C'ÉTAIT TOTALEMENT RAISON !");
        if (masterGain) masterGain.gain.value = 0.5;
        setPhase('chill');
    }, 2000);
});

btnAmtenael.addEventListener('click', () => {
    setPhase('amtenael');
    hypeLevel = 80;
    hypeProgress.style.width = '80%';
    
    // Zoom effect on DJ
    djAvatar.style.transform = 'scale(1.5)';
    setTimeout(() => djAvatar.style.transform = 'scale(1)', 500);
});

// Random subtitle rotation in phases
setInterval(() => {
    if (Math.random() > 0.7 && currentPhase !== 'chill') {
        const lines = SUBTITLES[currentPhase];
        updateSubtitle(lines[Math.floor(Math.random() * lines.length)]);
    }
}, 3000);
