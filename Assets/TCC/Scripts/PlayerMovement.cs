using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5;
    public GameObject PlayerModel;
    private void Start()
    {
        PlayerModel.SetActive(false);
        enabled = false;
    }

    public void SetPosition()
    {
        transform.position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        transform.position += dir.normalized * (Time.deltaTime * speed);
    }
}
