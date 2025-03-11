using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AutoGrab : MonoBehaviour
{
    public XRDirectInteractor handInteractor;
    private XRGrabInteractable grabInteractable;
    // Start is called before the first frame update
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

    }

    // Update is called once per frame
    void Update()
    {
        if (handInteractor && grabInteractable) {
            handInteractor.StartManualInteraction(grabInteractable);
        }
    }
}
