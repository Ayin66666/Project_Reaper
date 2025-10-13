using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_WaveExplosion : MonoBehaviour
{
    [Header("---Setting---")]
    public Type type;
    [SerializeField] private float startAngle = 90f;
    [SerializeField] private float endAngle = 270f;
    [SerializeField] private int bulletCount;
    private float speed;
    private Vector2 bulletDir;

    public enum Type { None, White, Black }
    [SerializeField] private GameObject[] bullet;

    public void ExplosionSetting(Type type, float speed)
    {
        this.speed = speed;
        this.type = type;
    }

    private void OnDestroy()
    {
        Semicircle();
    }

    private void Semicircle()
    {
        float angleStep = (endAngle - startAngle) / bulletCount;
        float angle = startAngle;

        for (int i = 0; i < bulletCount + 1; i++)
        {
            float bulDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
            float bulDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);
            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
            Vector2 bulDir = (bulMoveVector - transform.position).normalized;

            switch (type)
            {
                case Type.None:
                    GameObject obj = Instantiate(bullet[0], transform.position, Quaternion.identity);
                    obj.transform.rotation = Quaternion.FromToRotation(Vector3.up, bulletDir);
                    obj.GetComponent<Rigidbody2D>().velocity = bulDir * speed;
                    break;

                case Type.White:
                    GameObject obj1 = Instantiate(bullet[0], transform.position, Quaternion.identity);
                    obj1.transform.rotation = Quaternion.FromToRotation(Vector3.up, bulletDir);
                    obj1.GetComponent<Rigidbody2D>().velocity = bulDir * speed;
                    break;

                case Type.Black:
                    GameObject obj2 = Instantiate(bullet[0], transform.position, Quaternion.identity);
                    obj2.transform.rotation = Quaternion.FromToRotation(Vector3.up, bulletDir);
                    obj2.GetComponent<Rigidbody2D>().velocity = bulDir * speed;
                    break;
            }

            angle += angleStep;
        }
    }
}
