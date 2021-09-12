using LSHGame.PlayerN;
using SceneM;
using UnityEngine;
using UnityEngine.Events;
using LSHGame.Util;

namespace LSHGame
{
    [RequireComponent(typeof(PlayerFollower))]
    [RequireComponent(typeof(RecreateModule))]
    [RequireComponent(typeof(ReTransform))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(EffectsController))]
    public class CoinPickup : DataPersistBehaviour,IRecreatable,IRecreateBlocker
    {
        [SerializeField] private InventoryItem inventoryItem;
        [SerializeField]
        private UnityEvent onPickUp;

        [SerializeField]
        private LayerMask layermask;

        [SerializeField]
        private EffectTrigger activeFX;

        private PlayerFollower followComponent;
        private Animator animator;

        private bool destroied = false;

        private bool active = false;

        private void Awake()
        {
            followComponent = GetComponent<PlayerFollower>();
            animator = GetComponent<Animator>();
            
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if(layermask.IsLayer(collision.gameObject.layer))
                Activate();
        }

        private void Activate()
        {
            if (!destroied)
            {
                active = true;
                followComponent.Active = true;
                animator.SetTrigger("Activate");
                activeFX.Trigger();
                TryPickup();
            }
        }

        private void Update()
        {
            TryPickup();
        }

        private void TryPickup()
        {
            if(!destroied && active && Player.Instance.IsSaveGround)
            {
                followComponent.Active = false;
                Inventory.Add(inventoryItem);
                destroied = true;
                Destroy(gameObject);
                onPickUp?.Invoke();
            }
        }

        public override void LoadData(SceneM.Data data)
        {
            if(data is Data<bool> storedData && storedData.value)
            {
                Destroy(gameObject);
                //gameObject.SetActive(false);
            }
        }

        public override SceneM.Data SaveData()
        {
            return new Data<bool>(destroied);
        }

        public bool DoesRecreate() => !destroied;

        public void Recreate()
        {
            if (!destroied)
            {
                followComponent.Active = false;
                active = false;
            }
        }
    } 
}
