using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

enum REBEL_BASE_SELEC {ONE, TWO, STARTING}

public class ProbeDeck : MonoBehaviour {

	public Camera impCamera;
	public Camera rebCamera;
	public List <string> probeCards; // All probe cards in game
	public List <string> impStarting; //All Possible Imperial Starting Places
	public List <string> rebStarting; //All Possible Rebel Starting Places
	public List <string> impCards; //Probe Cards in Imperial Hand.
	public List <string> rebCards; //Probe Cards in Rebel Hand
	public List <string> rebBaseLocations; //Probe Cards available for moving rebel base to
	public string rebelBase; //The Rebel Base System
	public bool isTesting = false;

	// Use this for initialization
	void Awake () {
		probeCards = new List<string> ();
		impCards = new List<string> ();
		rebCards = new List<string> ();
		rebBaseLocations = new List<string> ();
		impStarting = new List<string> ();
		rebStarting = new List<string> ();

		LoadCards ();
		ShuffleProbeCards ();
		DrawInitialRebCards ();
		DrawInitialImperialCards ();

		if (isTesting) {
			TestLoadCards ();
			TestRebelInitialCards ();
			TestImpInitialCards ();
			TestRebelBase ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void LoadCards(){
		/* Loads ALl probecards from SystemNames.xml
		 * Sorts Cards into respective piles
		 */ 
		TextAsset starSystems = Resources.Load ("SystemNames") as TextAsset;
		using (XmlReader reader = XmlReader.Create (new StringReader (starSystems.text))) {
			XmlWriterSettings ws = new XmlWriterSettings ();
			ws.Indent = true;
			while (reader.Read ()) {
				switch (reader.NodeType) {
				case XmlNodeType.Element:
					switch (reader.Name) {
					case "Imperial":
						impStarting.Add (reader.GetAttribute ("Name"));
						probeCards.Add (reader.GetAttribute ("Name"));
						break;

					case "Rebel":
						rebStarting.Add (reader.GetAttribute ("Name"));
						probeCards.Add (reader.GetAttribute ("Name"));
						break;

					case "None":
						probeCards.Add (reader.GetAttribute ("Name"));
						break;
					}
				break;
				}
			}
		}
	}

	/*	This Block Handles all Rebel Player Interactions with probecards:
	 * 			Get Initial Systems
	 * 			Draw for Rebel Base and clearing list after it has been chosen
	 * 			Setting rebel base
	 * 
	 */ 

	private void DrawInitialRebCards(){
		/* Draws the Necessary Number of rebel starting Locations and adds them to hand
		 */ 
		List<int> selectedNumbers = new List<int> ();
		int i = 0;
		while (i < 3){
			int j = Random.Range (0, rebStarting.Count);
			if (!selectedNumbers.Contains (j)) {
				rebCards.Add (rebStarting [j]);
				selectedNumbers.Add (j);
				i++;
			}
		}
	}

	private void DrawRebelBaseCards(REBEL_BASE_SELEC type){
		/*	Draws the number of cards based upon the type of draw given
		 * 
		 */ 
		if (type == REBEL_BASE_SELEC.ONE)
			RebelBaseDraw (3);
		if (type == REBEL_BASE_SELEC.TWO)
			RebelBaseDraw (5);
		if (type == REBEL_BASE_SELEC.STARTING)
			RebelBaseDraw (probeCards.Count);
	}

	private void RebelBaseDraw(int num){
		/*	Draws the Cards up to the number given and adds them to the rebel players possible base locations
		 * 
		 */ 
		List<int>selectedNumbers = new List<int>();
		int i = 0;
		while (i < num){
			int j = Random.Range(0, probeCards.Count);
			if (!selectedNumbers.Contains (j)) {
				rebBaseLocations.Add (probeCards [j]);
				selectedNumbers.Add (j);
				i++;
			}
		}
	}

	private void SetRebelBase(string name){
		/*	Sets the rebel base and removes that probe card from the deck
		 * 
		 */ 
		rebelBase = name;
		probeCards.Remove (name);
	}

	private void ClearRebelBaseLocations(){
		rebBaseLocations.Clear();
	}

	/*This Block handles removing Probe Cards from the imperial Players hand 
	 * 
	 * 
	 */

	private void RemoveImperialCard(string name){
		impCards.Remove (name);
		probeCards.Add (name);
		ShuffleProbeCards ();
	}

	/*	This Block handles all actions Imperial Player does with Probe Cards:
	 * 			Draws Imperial Starting planets
	 * 			Draw A probe card from the deck
	 */ 

	private void DrawInitialImperialCards(){
		/*	Draws Five Systems for the Imperial Player to start with from available cards
		 *  in impStarting
		 * 	Removes those cards from the Probe deck
		 */ 
		List<int> selectedNumbers = new List<int> ();
		int i = 0;
		while (i < 5) {
			int j = Random.Range (0, impStarting.Count);
			if (!selectedNumbers.Contains (j)) {
				impCards.Add (impStarting [j]);
				selectedNumbers.Add (j);
				probeCards.Remove (impStarting [j]); // should do this after
				i++;
			}
		}
	}

	private void ImpDrawProbeCard(){
		/*	Draws one Random Card from the probe deck, adds it to imperial hand
		 *  and removes it from the probe deck.
		 */ 
		int i = Random.Range (0, probeCards.Count);
		impCards.Add (probeCards [i]);
		probeCards.RemoveAt(i);
	}

	/*	This Block handles testing for Above Code based upon boolean isTesting
	 * 
	 */ 

	private void TestLoadCards(){
		Debug.Log ("Test All Probe Cards in list");
		foreach (string s in probeCards)
			Debug.Log (s + "\n-----\n");
		Debug.Log ("Test Imperial Starting Systems");
		foreach (string s in impStarting)
			Debug.Log (s + "\n------\n");
		Debug.Log ("Test Rebel Starting Systems");
		foreach (string s in rebStarting)
			Debug.Log (s + "\n------\n");
	}

	private void TestRebelInitialCards(){
		Debug.Log ("Test Rebel Initial Systems");
		foreach (string s in rebCards)
			Debug.Log (s + "\n-------\n");
	}

	private void TestImpInitialCards(){
		Debug.Log ("Test Imp Initial Systems");
		foreach (string s in impCards)
			Debug.Log (s + "\n--------\n");
	}

	private void TestRebelBase(){
		Debug.Log ("Test Rebel Base Selection");
		SetRebelBase ("Bespin");
		Debug.Log ("After setting to Bespin: " + rebelBase + " does probe deck contain Bespin? " + probeCards.Contains ("Bespin"));
	}

	private void ShuffleProbeCards(){
		/* from http://answers.unity3d.com/questions/486626/how-can-i-shuffle-alist.html
		 *  Based on Fisher-Yates shuffle
		 */ 
		int n = probeCards.Count;
		for (int i = 0; i < n; i++) {
			int r = i + (int)(Random.Range (0.0f, 1.0f) * (n - i - 1));
			string temp = probeCards [r];
			probeCards [r] = probeCards [i];
			probeCards [i] = temp;
		}
	}
}
