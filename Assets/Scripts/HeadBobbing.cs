using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    public float bobbingSpeed = 14f;
    public float bobbingAmount = 0.028f;
    public float returnSpeed = 1.5f;

    private float defaultYPosition;
    private float timer = 0f;

    void Start()
    {
        defaultYPosition = transform.localPosition.y;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            timer += Time.deltaTime * bobbingSpeed;
            float newY = defaultYPosition + Mathf.Sin(timer) * bobbingAmount;
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        }
        else
        {
            timer = 0f;
            float smoothY = Mathf.Lerp(transform.localPosition.y, defaultYPosition, Time.deltaTime * returnSpeed);
            transform.localPosition = new Vector3(transform.localPosition.x, smoothY, transform.localPosition.z);
        }
    }
    
}