﻿using MalbersAnimations.Reactions;
using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations
{
    [DisallowMultipleComponent]
    /// <summary> Damager Receiver</summary>
    [AddComponentMenu("Malbers/Damage/MDamageable")]
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/secondary-components/mdamageable")]
    public class MDamageable : MonoBehaviour, IMDamage
    {
        [Tooltip("Animal Reaction to apply when the damage is done")]
        public Component character;

        [Tooltip("Animal Reaction to apply when the damage is done")]
        [SerializeReference, SubclassSelector]
        public Reaction reaction;

        [Tooltip("Animal Reaction when it receives a critical damage")]
        [SerializeReference, SubclassSelector]
        public Reaction criticalReaction;

        [Tooltip("Reaction sent to the Damager if it hits this Damageable")]
        [SerializeReference, SubclassSelector]
        public Reaction damagerReaction;


        [Tooltip("The Damageable will ignore the Reaction coming from the Damager. Use this when this Damager Needs to have the Default Reaction")]
        [SerializeField] private BoolReference ignoreDamagerReaction = new BoolReference(false);

        [Tooltip("Stats component to apply the Damage")]
        public Stats stats;

        [Tooltip("Multiplier for the Stat modifier Value. Use this to increase or decrease the final value of the Stat")]
        public FloatReference multiplier = new FloatReference(1);

        [Tooltip("When Enabled the animal will rotate towards the Damage direction"), UnityEngine.Serialization.FormerlySerializedAs("AlingToDamage")]
        public BoolReference AlignToDamage = new BoolReference(false);

        [Tooltip("Time to align to the damage direction")]
        public FloatReference AlignTime = new FloatReference(0.25f);
        [Tooltip("Aligmend curve")]
        public AnimationCurve AlignCurve = new AnimationCurve(MTools.DefaultCurve);

        public MDamageable Root;
        public damagerEvents events;

        public Vector3 HitDirection { get; set; }
        public GameObject Damager { get; set; }
        public GameObject Damagee => gameObject;

        public bool IgnoreDamagerReaction { get => ignoreDamagerReaction; set => ignoreDamagerReaction.Value = value; }

        public DamageData LastDamage;

        [Tooltip("Elements that affect the MDamageable")]
        public List<ElementMultiplier> elements = new List<ElementMultiplier>();

        private MDamageableProfile Default;

        private string currentProfileName = "Default";


        [Tooltip("The Damageable can Change profiles to Change the way the Animal React to the Damage")]
        public List<MDamageableProfile> profiles = new List<MDamageableProfile>();


        [HideInInspector] public int Editor_Tabs1;

        private void Start()
        {
            if (character == null && reaction != null)
                character = stats.GetComponent(reaction.ReactionType); //Find the character where the Stats are
            else
            {
                character = stats.transform;
            }

            //Store the default values
            Default = new MDamageableProfile("Default", reaction,criticalReaction, damagerReaction, ignoreDamagerReaction, multiplier, AlignToDamage, elements);

            if (profiles == null) profiles = new List<MDamageableProfile>(); //Unullify the profiles
        }


        /// <summary> Restore the Default Damageable profile </summary>
        public virtual void Profile_Restore()
        {
            reaction = Default.reaction;
            damagerReaction = Default.DamagerReaction;
            multiplier = Default.multiplier;
            AlignToDamage = Default.AlignToDamage;
            elements = Default.elements;
            ignoreDamagerReaction = Default.ignoreDamagerReaction;
            criticalReaction = Default.criticalReaction;
        }

        public virtual MDamageableProfile GetCurrentProfile()
        {
            var Damagprof = new MDamageableProfile()
            {
                name = this.currentProfileName,
                AlignToDamage = this.AlignToDamage,
                DamagerReaction = this.damagerReaction,
                elements = this.elements,
                ignoreDamagerReaction = this.ignoreDamagerReaction,
                multiplier = this.multiplier,
                reaction = this.reaction,
                criticalReaction = this.criticalReaction,
            };
            return Damagprof;
        }

        public virtual void Profile_Set(string name)
        {
            if (string.IsNullOrEmpty(name) || name.ToLower() == "default")
            {
                Profile_Restore();
            }
            else
            {
                var index = profiles.FindIndex(p => p.name == name);

                if (index != -1)
                {
                    var D = profiles[index];
                    currentProfileName = D.name;
                    reaction = D.reaction;
                    damagerReaction = D.DamagerReaction;
                    ignoreDamagerReaction = D.ignoreDamagerReaction;
                    multiplier = D.multiplier;
                    AlignToDamage = D.AlignToDamage;
                    elements = D.elements;
                    criticalReaction = D.criticalReaction;
                }
            }
        }



        //-*********************************************************************--
        /// <summary>  Main Receive Damage Method!!! </summary>
        /// <param name="Direction">The Direction the Damage is coming from</param>
        /// <param name="Damager">Game Object doing the Damage</param>
        /// <param name="damage">Stat Modifier containing the Stat ID, what to modify and the Value to modify</param>
        /// <param name="isCritical">is the Damage Critical?</param>
        /// <param name="react">Does the Damage that is coming has a Custom Reaction? </param>
        /// <param name="customReaction">The Attacker Brings a custom Reaction to override the Default one</param>
        /// <param name="pureDamage"></param>
        /// <param name="element"></param>
        public virtual void ReceiveDamage(Vector3 Direction, GameObject Damager, StatModifier damage,
            bool isCritical, bool react, Reaction customReaction, bool pureDamage, StatElement element)
        {
            if (!enabled) return;       //This makes the Animal Immortal.
            HitDirection = Direction;   //IMPORTANT!!! to React

            if (react)
            {
                //Custom reaction if the Attacker sends one and Ignore Damager is False
                if (customReaction != null && !IgnoreDamagerReaction)
                {
                    if (!customReaction.TryReact(character)) //if there's no valid reaction then use the default one
                    {
                        DoReaction(isCritical);
                    }
                }
                else
                {
                    DoReaction(isCritical);
                }
            }


            //Make the Damager react to the Damageable
            if (Damager) damagerReaction?.React(Damager);
            



            var stat = stats.Stat_Get(damage.ID);
            if (stat == null || !stat.Active || stat.IsEmpty || stat.IsInmune) return; //Do nothing if the stat is empty, null or disabled

            ElementMultiplier statElement = new ElementMultiplier(element, 1);


            //Apply the Element Multiplier
            if (element != null && elements.Count > 0)
            {
                statElement = elements.Find(x => element.ID == x.element.ID);
                damage.Value *= statElement.multiplier;
                events.OnElementDamage.Invoke(statElement.element.ID);
                Root?.events.OnElementDamage.Invoke(statElement.element.ID);
            }

            SetDamageable(Direction, Damager);
            Root?.SetDamageable(Direction, Damager);                     //Send the Direction and Damager to the Root 


            //Store the last damage applied to the Damageable
            LastDamage = new DamageData(Damager, gameObject, damage, isCritical, statElement);
            if (Root) Root.LastDamage = LastDamage;


            if (isCritical)
            {
                events.OnCriticalDamage.Invoke();
                Root?.events.OnCriticalDamage.Invoke();
            }

            if (!pureDamage)
                damage.Value *= multiplier;               //Apply to the Stat modifier a new Modification

            events.OnReceivingDamage.Invoke(damage.Value);
            events.OnDamager.Invoke(Damager);

            //Send the Events on the Root
            Root?.events.OnReceivingDamage.Invoke(damage.Value);
            Root?.events.OnDamager.Invoke(Damager);

            damage.ModifyStat(stat);

            if (AlignToDamage.Value)
            {
                AlignToDamageDirection(Damager);
            }
        }

        private void DoReaction(bool isCritical)
        {
            if (isCritical)
                criticalReaction?.React(character);     //if the damage is Critical then react with the critical reaction instead
            else
                reaction?.React(character);    //React Default
        }

        private void AlignToDamageDirection(GameObject Direction)
        {
            if (!Direction.IsDestroyed() )
            StartCoroutine(MTools.AlignLookAtTransform(character.transform, Direction.transform.position, AlignTime.Value, AlignCurve));
        }

        /// <summary>  Receive Damage from external sources simplified </summary>
        /// <param name="stat"> What stat will be modified</param>
        /// <param name="amount"> value to substact to the stat</param>
        public virtual void ReceiveDamage(StatID stat, float amount)
        {
            var modifier = new StatModifier() { ID = stat, modify = StatOption.SubstractValue, Value = amount };
            ReceiveDamage(Vector3.forward, null, modifier, false, true, null, false, null);
        }

        public virtual void ReceiveDamage(StatID stat, float amount, Vector3 direction, bool alignOverride, GameObject damager)
        {
            bool temp = AlignToDamage;
            AlignToDamage.Value = alignOverride;
            ReceiveDamage(direction, damager, stat, amount, StatOption.SubstractValue);
            AlignToDamage.Value = temp;
        }

        /// <summary>  Receive Damage from external sources simplified </summary>
        /// <param name="stat"> What stat will be modified</param>
        /// <param name="amount"> value to substact to the stat</param>
        public virtual void ReceiveDamage(StatID stat, float amount, StatOption modifyStat = StatOption.SubstractValue)
        {
            var modifier = new StatModifier() { ID = stat, modify = modifyStat, Value = amount };
            ReceiveDamage(Vector3.forward, null, modifier, false, true, null, false, null);
        }



        /// <summary>  Receive Damage from external sources simplified </summary>
        /// <param name="Direction">Where the Damage is coming from</param>
        /// <param name="Damager">Who is doing the Damage</param>
        /// <param name="modifier">What Stat will be modified</param>
        /// <param name="modifyStat">Type of modification applied to the stat</param>
        /// <param name="isCritical">is the Damage Critical?</param>
        /// <param name="react">Does Apply the Default Reaction?</param>
        /// <param name="pureDamage">if is pure Damage, do not apply the default multiplier</param>
        /// <param name="stat"> What stat will be modified</param>
        /// <param name="amount"> value to substact to the stat</param>
        public virtual void ReceiveDamage(Vector3 Direction, GameObject Damager, StatID stat, float amount, StatOption modifyStat = StatOption.SubstractValue,
             bool isCritical = false, bool react = true, Reaction customReaction = null, bool pureDamage = false, StatElement element = null)
        {
            var modifier = new StatModifier() { ID = stat, modify = modifyStat, Value = amount };
            ReceiveDamage(Direction, Damager, modifier, isCritical, react, customReaction, pureDamage, element);
        }


        /// <summary>  Receive Damage from external sources simplified </summary>
        /// <param name="Direction">Where the Damage is coming from</param>
        /// <param name="Damager">Who is doing the Damage</param>
        /// <param name="modifier">What Stat will be modified</param>
        /// <param name="isCritical">is the Damage Critical?</param>
        /// <param name="react">Does Apply the Default Reaction?</param>
        /// <param name="pureDamage">if is pure Damage, do not apply the default multiplier</param>
        /// <param name="stat"> What stat will be modified</param>
        /// <param name="amount"> value to substact to the stat</param>
        public virtual void ReceiveDamage(Vector3 Direction, GameObject Damager, StatID stat,
            float amount, bool isCritical = false, bool react = true, Reaction customReaction = null, bool pureDamage = false)
        {
            var modifier = new StatModifier() { ID = stat, modify = StatOption.SubstractValue, Value = amount };
            ReceiveDamage(Direction, Damager, modifier, isCritical, react, customReaction, pureDamage, null);
        }


        /// <summary>  Receive Damage from external sources simplified </summary>
        /// <param name="Direction">Where the Damage is coming from</param>
        /// <param name="Damager">Who is doing the Damage</param>
        /// <param name="modifier">What Stat will be modified</param>
        /// <param name="isCritical">is the Damage Critical?</param>
        /// <param name="react">Does Apply the Default Reaction?</param>
        /// <param name="pureDamage">if is pure Damage, do not apply the default multiplier</param>
        /// <param name="stat"> What stat will be modified</param>
        /// <param name="amount"> value to substact to the stat</param>
        public virtual void ReceiveDamage(Vector3 Direction, GameObject Damager, StatModifier damage,
        bool isCritical, bool react, Reaction customReaction, bool pureDamage) =>
         ReceiveDamage(Direction, Damager, damage, isCritical, react, customReaction, pureDamage, null);

        internal void SetDamageable(Vector3 Direction, GameObject Damager)
        {
            HitDirection = Direction;
            this.Damager = Damager;
        }

        [System.Serializable]
        public class damagerEvents
        {
            public FloatEvent OnReceivingDamage = new FloatEvent();
            public UnityEvent OnCriticalDamage = new UnityEvent();
            public GameObjectEvent OnDamager = new GameObjectEvent();
            public IntEvent OnElementDamage = new IntEvent();
        }

        public struct DamageData
        {
            /// <summary>  Who made the Damage ? </summary>
            public GameObject Damager;
            /// <summary>  Who made the Damage ? </summary>
            public GameObject Damagee;
            /// <summary>  Final Stat Modifier ? </summary>
            public StatModifier stat;


            /// <summary> Final value who modified the Stat</summary>
            public float Damage => stat.modify != StatOption.None ? stat.Value  : 0f;

            /// <summary>Store if the Damage was Critical</summary>
            public bool WasCritical;

            /// <summary>Store if the damage was  </summary>
            public ElementMultiplier Element;

            public DamageData(GameObject damager, GameObject damagee, StatModifier stat, bool wasCritical, ElementMultiplier element)
            {
                Damager = damager;
                Damagee = damagee;
                this.stat = new StatModifier(stat);
                WasCritical = wasCritical;
                Element = element;
            }
        }


#if UNITY_EDITOR
        private void Reset()
        {
            // reaction = MTools.GetInstance<ModeReaction>("Damaged");

            reaction = new ModeReaction()
            {
                ID = MTools.GetInstance<ModeID>("Damage"),
            };
            stats = this.FindComponent<Stats>();
            Root = transform.FindComponent<MDamageable>();     //Check if there's a Damageable on the Root
            if (Root == this) Root = null;


            //Add Stats if it not exist
            if (stats == null) stats = gameObject.AddComponent<Stats>();

            profiles = new List<MDamageableProfile>();
        }

        [HideInInspector] public bool First_Change = false;

        private void OnValidate()
        {
            if (reaction == null && !First_Change)
            {
                reaction = new ModeReaction()
                {
                    ID = MTools.GetInstance<ModeID>("Damage"),
                };
                First_Change = true;
                MTools.SetDirty(this);
            }
        }
#endif
    }


    [System.Serializable]
    public struct MDamageableProfile
    {
        [Tooltip("Name of the Profile. This is used for Setting a New Damageable Profile. E.g. When the Animal is blocking or Parrying")]
        public string name;

        [Tooltip("Animal Reaction to apply when the damage is done")]
        [SerializeReference, SubclassSelector]
        public Reaction reaction;

        [Tooltip("Animal Reaction when it receives a critical damage")]
        [SerializeReference, SubclassSelector]
        public Reaction criticalReaction;

        [Tooltip("Animal Reaction to apply when the damage is done")]
        [SerializeReference, SubclassSelector]
        public Reaction DamagerReaction;

        [Tooltip("Multiplier for the Stat modifier Value. Use this to increase or decrease the final value of the Stat")]
        public FloatReference multiplier;

        [Tooltip("When Enabled the animal will rotate towards the Damage direction")]
        public BoolReference AlignToDamage;

        public BoolReference ignoreDamagerReaction;

        [Tooltip("Elements that affect the MDamageable")]
        public List<ElementMultiplier> elements;

        public MDamageableProfile(string Name, Reaction reaction, Reaction criticalReaction, Reaction DamagerReaction, BoolReference ignoreDamagerReaction,
            FloatReference multiplier, BoolReference AlignToDamage, List<ElementMultiplier> elements)
        {
            this.name = Name;
            this.reaction = reaction;
            this.DamagerReaction = DamagerReaction;
            this.multiplier = multiplier; 
            this.criticalReaction = criticalReaction;
            this.AlignToDamage = AlignToDamage;
            this.elements = elements;
            this.ignoreDamagerReaction = ignoreDamagerReaction;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MDamageable))]
    public class MDamageableEditor : Editor
    {
        SerializedProperty reaction, damagerReaction, criticalReaction, 
            stats, 
            multiplier, ignoreDamagerReaction, events, Root, AlignTime, AlignCurve, AlignToDamage, Editor_Tabs1, elements, profiles;
        MDamageable M;

        protected string[] Tabs1 = new string[] { "General", "Profiles", "Events" };


        GUIContent plus;

        private void OnEnable()
        {
            M = (MDamageable)target;

            reaction = serializedObject.FindProperty("reaction");
            criticalReaction = serializedObject.FindProperty("criticalReaction");
            damagerReaction = serializedObject.FindProperty("damagerReaction");
            stats = serializedObject.FindProperty("stats");
            multiplier = serializedObject.FindProperty("multiplier");
            events = serializedObject.FindProperty("events");
            Root = serializedObject.FindProperty("Root");
            AlignToDamage = serializedObject.FindProperty("AlignToDamage");
            AlignCurve = serializedObject.FindProperty("AlignCurve");
            AlignTime = serializedObject.FindProperty("AlignTime");
            Editor_Tabs1 = serializedObject.FindProperty("Editor_Tabs1");
            elements = serializedObject.FindProperty("elements");
            profiles = serializedObject.FindProperty("profiles");
            ignoreDamagerReaction = serializedObject.FindProperty("ignoreDamagerReaction");

            if (plus == null) plus = UnityEditor.EditorGUIUtility.IconContent("d_Toolbar Plus");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            MalbersEditor.DrawDescription("Allows the Animal React and Receive damage from external sources");


            Editor_Tabs1.intValue = GUILayout.Toolbar(Editor_Tabs1.intValue, Tabs1);

            switch (Editor_Tabs1.intValue)
            {
                case 0: DrawGeneral(); break;
                case 1: DrawProfiles(); break;
                case 2: DrawEvents(); break;
                default: break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawProfiles()
        {
            //using (new GUILayout.HorizontalScope())
            // {
            EditorGUILayout.PropertyField(profiles, true);

            //if (GUILayout.Button(plus, UnityEditor.EditorStyles.helpBox))
            //{
            //    profiles.InsertArrayElementAtIndex(profiles.arraySize);
            //    serializedObject.ApplyModifiedProperties();
            //}
            // }
        }

        private void DrawGeneral()
        {
            if (M.transform.parent != null)
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.PropertyField(Root);
                }
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(reaction, new GUIContent("Default Reaction"));
                EditorGUILayout.PropertyField(criticalReaction);
                EditorGUILayout.PropertyField(damagerReaction);
                EditorGUI.indentLevel--;
                EditorGUILayout.PropertyField(ignoreDamagerReaction);
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(stats);
                EditorGUILayout.PropertyField(multiplier);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(elements);
                EditorGUI.indentLevel--;
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(AlignToDamage);

                if (M.AlignToDamage.Value)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PropertyField(AlignTime);
                        EditorGUILayout.PropertyField(AlignCurve, GUIContent.none, GUILayout.MaxWidth(75));
                    }
                }
            }
        }


        private void DrawEvents()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(events, true);
                EditorGUI.indentLevel--;
            }
        }


    }
#endif
}