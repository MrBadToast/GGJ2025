using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PatternDirection
{
    Right,
    Left
}

public class PatternGenerator : StaticSerializedMonoBehaviour<PatternGenerator>
{
    public class Pattern
    {
        public GameObject patternObject;
        public PatternDirection direction;
        public float leastTime = 0f;
        public int spawnRate = 1;
    }

    public class InstanciatedPattern
    {
        public GameObject patternObject;
        public PlatformPattern PatternComp { get { return patternObject.GetComponent<PlatformPattern>(); } }

        static public GameObject InstanciateObject(Pattern original, Vector2 position)
        {
            return Instantiate(original.patternObject, position, Quaternion.identity);
        }
    }

    [SerializeField] private Pattern[] assignedPatterns;
    [SerializeField] private float generateOffset = 1f;
    [SerializeField] private List<InstanciatedPattern> activePatterns;

    [SerializeField] private Vector2 lastGeneratedEnd = Vector2.zero;
    public Vector2 LastGeneratedEnd { get { return lastGeneratedEnd; } }
    private PatternDirection lastDirection = PatternDirection.Right;

    protected override void Awake()
    {
        base.Awake();
        activePatterns = new List<InstanciatedPattern>();
    }

    public Pattern[] GetValidPatterns(float currentTime)
    {
        List<Pattern> result = new List<Pattern>();

        foreach (Pattern pat in assignedPatterns)
        {
            if (pat.leastTime <= currentTime)
                result.Add(pat);
        }

        if (result.Count == 0) Debug.LogError("THERES NO VALID PATTERNS");

        return result.ToArray();
    }

    public Pattern PickRandomFromArray(Pattern[] array)
    {
        int sum = 0;

        for (int i = 0; i < array.Length; i++)
        {
            sum += array[i].spawnRate;
        }

        int picked = Random.Range(0, sum+1);
        int currentIndex = 0;

        Pattern result = new Pattern();

        foreach (var pat in array)
        {
            currentIndex += pat.spawnRate;

            if (picked <= currentIndex)
            {
                result = pat;
                break;
            }
        }

        return result;
    }

    [Button()]
    public void GeneratePattern(float currentTime)
    {
        PatternDirection lastDirection = activePatterns.Count == 0 ?
            PatternDirection.Right :
            activePatterns[activePatterns.Count-1].PatternComp.GetNextDirection();

        Pattern[] valid = GetValidPatterns(currentTime);
        Pattern patternToGenerate = PickRandomFromArray(valid);

        InstanciatedPattern instanciated = new InstanciatedPattern();
        instanciated.patternObject = InstanciatedPattern.InstanciateObject(patternToGenerate, new Vector2( 0f,lastGeneratedEnd.y + generateOffset));
        activePatterns.Add(instanciated);

        lastGeneratedEnd = instanciated.PatternComp.EndPointPositon;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(lastGeneratedEnd, 0.5f);
    }

}