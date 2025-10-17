using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Enemy_WaveExplosion : MonoBehaviour
{
    [Header("---Setting---")]
    public Type type;
    [SerializeField] private LayerMask groundLayer;
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 10f, groundLayer);
        if (hit.collider != null) transform.position = hit.point;
        transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);

        float angleStep = (endAngle - startAngle) / bulletCount;
        float angle = startAngle;

        for (int i = 0; i < bulletCount + 1; i++)
        {
            float bulDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
            float bulDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);
            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
            Vector2 bulDir = (bulMoveVector - transform.position).normalized;

            // Bullet Spawn
            switch (type)
            {
                case Type.None:
                    GameObject obj = Instantiate(bullet[0], transform.position, Quaternion.identity);
                    obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.Blue, bulDir, 5, 30, 10);
                    break;

                case Type.White:
                    GameObject obj1 = Instantiate(bullet[0], transform.position, Quaternion.identity);
                    obj1.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.Blue, bulDir, 5, 30, 10);
                    break;

                case Type.Black:
                    GameObject obj2 = Instantiate(bullet[0], transform.position, Quaternion.identity);
                    obj2.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.Blue, bulDir, 5, 30, 10);
                    break;
            }

            angle += angleStep;
        }
    }
}
