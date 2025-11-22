using UnityEngine;

public class BodyTracker : MonoBehaviour
{
    // Cette méthode sera appelée par le Membre 2 à chaque frame avec les données IMU
    public void UpdateFromIMU(Quaternion rotation, Vector3 acceleration = default)
    {
        // Rotation du corps (calculée via filtre Madgwick/Kalman)
        transform.rotation = rotation;

        // Optionnel : petite translation si tu veux simuler le déplacement du centre de gravité
        // (ex. lors d’un squat, le centre descend un peu)
        // transform.position = new Vector3(0, calculatedHeight, 0);
    }

    // Pour tester tout de suite dans l’éditeur (tu pourras supprimer après)
    void Update()
    {
        if (Application.isEditor)
        {
            // Simulation simple : rotation lente pour tester la caméra
            transform.rotation = Quaternion.Euler(0, Mathf.Sin(Time.time * 30) * 30, 0);
        }
    }
}