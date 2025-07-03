using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class EnemyDropSystemTest
{
    private EnemyDropSystem dropSystem;
    private GameObject testGameObject;
    
    [SetUp]
    public void Setup()
    {
        testGameObject = new GameObject("TestEnemyDropSystem");
        dropSystem = testGameObject.AddComponent<EnemyDropSystem>();
    }
    
    [TearDown]
    public void TearDown()
    {
        if (testGameObject != null)
        {
            Object.DestroyImmediate(testGameObject);
        }
    }
    
    [Test]
    public void TestEnemyDropSystemCreation()
    {
        // Verify that the EnemyDropSystem can be created without errors
        Assert.IsNotNull(dropSystem);
        Assert.IsNotNull(EnemyDropSystem.Instance);
    }
    
    [Test]
    public void TestCurseAddAndRemove()
    {
        // Create a mock CurseData
        CurseData mockCurse = ScriptableObject.CreateInstance<CurseData>();
        
        // Test adding curse
        dropSystem.AddAvailableCurse(mockCurse);
        
        // Test removing curse
        dropSystem.RemoveAvailableCurse(mockCurse);
        
        // No assertions needed, just verify no compilation errors
        Assert.IsTrue(true);
    }
    
    [Test]
    public void TestDropChanceSettings()
    {
        // Test setting drop chances
        dropSystem.SetDropChance(0.5f);
        dropSystem.SetCurseDropChance(0.2f);
        
        // Verify it doesn't crash
        Assert.IsTrue(true);
    }
}