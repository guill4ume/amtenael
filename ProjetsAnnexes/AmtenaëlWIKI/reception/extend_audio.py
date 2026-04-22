from pydub import AudioSegment
import os

def extend_audio(input_file, output_file, target_duration_ms=180000):
    print(f"Chargement de {input_file}...")
    if not os.path.exists(input_file):
        print(f"Erreur : Le fichier {input_file} n'existe pas.")
        return

    try:
        # Pydub supporte divers formats (mp3, wav, mp4) si FFmpeg est installé
        audio = AudioSegment.from_file(input_file)
    except Exception as e:
        print(f"Erreur de chargement : {e}")
        print("Assurez-vous d'avoir installé ffmpeg ! (Obligatoire pour les MP3/MP4)")
        return

    audio_length = len(audio)
    if audio_length == 0:
        print("Erreur : Le fichier audio est vide.")
        return

    print("Création de la boucle (crossfade de 500ms pour lisser la transition)...")
    
    # Crossfade plus long pour cacher la coupure du DJ (ex: 500ms)
    crossfade_duration = min(500, audio_length // 4) 
    
    extended_audio = audio
    while len(extended_audio) < target_duration_ms:
        extended_audio = extended_audio.append(audio, crossfade=crossfade_duration)

    # On coupe à 3 minutes
    extended_audio = extended_audio[:target_duration_ms]

    # Ajout d'un fondu de fermeture à la fin pour que ça ne coupe pas sec
    extended_audio = extended_audio.fade_out(3000)

    print(f"Sauvegarde en cours dans {output_file}...")
    extended_audio.export(output_file, format="mp3")
    print("\n✅ Terminé ! Ta version longue est prête.")

if __name__ == "__main__":
    print("=== GÉNÉRATEUR DE VERSION LONGUE ===")
    print("Instructions au préalable :")
    print("1. Avoir téléchargé la vidéo de Benjamin Fenêtre (en mp3 ou mp4).")
    print("2. Avoir installé pydub : pip install pydub")
    print("------------------------------------\n")
    
    input_path = input("Chemin de la piste originale (ex: original.mp3) : ").strip()
    # Enlève les guillemets potentiels si on fait un glisser-déposer sur le terminal
    input_path = input_path.strip('"\'') 
    
    output_path = "version_longue_dj_bluetooth.mp3"

    if input_path:
        extend_audio(input_path, output_path, target_duration_ms=180000) # 3 minutes
    else:
        print("Veuillez renseigner un chemin valide.")
