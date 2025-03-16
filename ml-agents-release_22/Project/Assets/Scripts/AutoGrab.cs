using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AutoGrab : MonoBehaviour
{
    public XRDirectInteractor handInteractor;
    public XRBaseController rightController;
    public float hapticStrength = 0.3f;
    public float hapticDuration = 0.05f;
    private XRGrabInteractable grabInteractable;
    // Start is called before the first frame update
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball")) {
            if (rightController != null) {
                rightController.SendHapticImpulse(hapticStrength, hapticStrength);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (handInteractor && grabInteractable) {
            handInteractor.StartManualInteraction(grabInteractable);
        }
    }
}
