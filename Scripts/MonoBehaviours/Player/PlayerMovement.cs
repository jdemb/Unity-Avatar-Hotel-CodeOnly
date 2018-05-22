using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI; //nowy namespace

public class PlayerMovement : MonoBehaviour {

	public Animator animator;
	public NavMeshAgent agent; //publiczne obiekty do wstawienia
	public float inputHoldDelay = 0.5f; //jak długie wait for seconds powinno być, jak długo nie możemy ruszać postacią po jednym kliknięciu
	public float turnSpeedThreshold = 0.5f; //jezeli speed tyle to moze zaczac skręcać
	public float speedDampTime = 0.1f; // bo chcemy płynną zmianę w prędkości, czas w jakim speed bedzie sie zmienial do nowej wartosci
	public float slowingSpeed = 0.175f; // jak szybko zwalniamy
	public float turnSmoothing = 15f; // jak szybko chcemy sie obracac, wieksza wartosc wieksza predkosc

	private WaitForSeconds inputHoldWait; //
	private Vector3 destinationPosition; // w jaką pozycję chcemy się udać, kiedy postac bedzie blisko bedziemy wywolywac stopping
	private const float stopDistanceProportion = 0.1f; 
	private readonly int hashSpeedParameter = Animator.StringToHash("Speed"); //string representing an integet - hash, musi pasowac do tego co wpisalismy w animatorze 
	private const float navMeshSampleDistance = 4f; // distance away from a click that the navmesh can be
	private Interactable currentInteractable; // interactable ktore kliknelismy
	private bool handleInput = true; // false kiedy cos robimy i chcemy zablokowac input
	private readonly int hashLocomotionTag = Animator.StringToHash("Locomotion"); // chcemy wiedziec czy jestesmy w okreslonym stanie Locomotion

	private void Start()
	{
		agent.updateRotation = false; // nie chcemy żeby agent obracał nam postacią, chcemy robić to sami

		inputHoldWait = new WaitForSeconds(inputHoldDelay); // coroutine do ustalenia pauzy pomiędzy interakcjami, zatrzymanie na chwilę inputu myszki 
		destinationPosition = transform.position; // kiedy first frame to speed nieustalone wiec postac bedzie uwieziona/zatrzasnieta w pierwotnej pozycji, dlatego trzeba to ustawic
	}

	private void OnAnimatorMove() // glowna funkcja jak postac bedzie sie ruszac
	{
		agent.velocity = animator.deltaPosition / Time.deltaTime; //navmesh agent moving character but at a rate determined by the animator
		// animator deltaposition s how far the character wants to move this frame, time delta time is time between frames
	}

	private void Update() 
	{
		if(agent.pathPending) // byc pewnym zeby nie robic gdy planujemy cos robic, calculating the path
		{
			return;
		}

		float speed = agent.desiredVelocity.magnitude; // based decision on how fast it wants to move, 

		//to teraz ktora z tych trzech chcemy wywolac stopping, slowing czy moving
		//first priority is to call stopping, very very close to destination, at least in stopping distance, 

		if(agent.remainingDistance <= agent.stoppingDistance * stopDistanceProportion)
		{
			Stopping(out speed);
		}
		else if(agent.remainingDistance <= agent.stoppingDistance)
		{
			Slowing(out speed, agent.remainingDistance); // trzeba tez podac remaining distance
		}
		else if(speed > turnSpeedThreshold) // moving if were going fast enough, if we are close to our destination we s
			//dont want to spin around really fast, najpierw moving potem turning
		{
			Moving();
		}
		animator.SetFloat(hashSpeedParameter, speed, speedDampTime, Time.deltaTime); // tell the animator how fast we are going

	}

	private void Stopping(out float speed) // calculate speed in this function, affect the speed, 
	{
		agent.isStopped = true; //prevent agent from moving
		transform.position = destinationPosition; // snap to that final place what we are aiming for
		speed = 0f; // bo nie chcemy sie ruszac

		if(currentInteractable) // jezeli na celu mamy interactable chcemy wiedziec ze idziemy w odpowiednim kierunku
		{
			transform.rotation = currentInteractable.interactionLocation.rotation;
			currentInteractable.Interact();
			currentInteractable = null;
			StartCoroutine(WaitforInteraction());
		}
	}

	private void Slowing(out float speed, float distanceToDestination) // slowing based on how far away from destination
	{
		agent.isStopped = true; // chcemy miec kontrole nad pozycja postaci, wiec agentowi wylaczamy
		transform.position = Vector3.MoveTowards(transform.position, destinationPosition, slowingSpeed * Time.deltaTime); // nwa pozycja postaci
		// wiec movetowards function, wiec zmieniamy pozycje postaci w kierunku celu i ustawiamy predkosc na podstawie nowej odleglosci
		float proportionalDistance = 1f - distanceToDestination / agent.stoppingDistance; // musimy wiedziec jak blisko jestesmy w porownaniu do stoppingdistancem, musimy znac dystans w proporcjach
		speed = Mathf.Lerp(slowingSpeed, 0f, proportionalDistance); // kiedy proportional distance jest 1 dostaniemy 0, kiedy distance to destination jest bardzo mala, to
		// ustawimy speed na bardzo mala wartosc, kiedy dystans do celu jest zblizony do agent.stoppingdistance to ustawimy nam predkosc bliska slowing speed 

		// zeby nam sie postac odrazu nie odwrocila tylko powoli
		Quaternion targetRotation = currentInteractable ? currentInteractable.interactionLocation.rotation : transform.rotation; // jezeli nie interactable to zostaw kierunek 
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, proportionalDistance);
	}

	private void Moving() // misleading name, wszystko tu jest moving, ale tu tylko ustawiamy kierunek, 
	{
		Quaternion targetRotation = Quaternion.LookRotation(agent.desiredVelocity);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSmoothing * Time.deltaTime);
	}

	public void OnGroundClick(BaseEventData data) // event trigger that gets called when the ground is clicked on, 
		// on groundclick will receive infomartion about whats happening wiec parameter
	{
		if (!handleInput)
		{
			return;
		}

		currentInteractable = null;

		PointerEventData pData = (PointerEventData)data;// chcemy tylko myszke i klikniecia ze wszystkich informacji
		NavMeshHit hit; // co bylo hit
		//to jak mamy point click to teraz chcemy znalezc punkt na navmesh ktory jest najblizej tego click
		if(NavMesh.SamplePosition(pData.pointerCurrentRaycast.worldPosition, out hit, navMeshSampleDistance, NavMesh.AllAreas))
		{
			//bo to zwraca tez boola czy pykło
			destinationPosition = hit.position; 
		}
		else
		{
			destinationPosition = pData.pointerCurrentRaycast.worldPosition; // player is going to try ands find its way to wherever the user clicked
		}
		agent.SetDestination(destinationPosition); // ustawiamy pozycje
		agent.isStopped = false; // przywracamy agenta do dzialania
	}

	public void OnInteractableClick(Interactable interactable)
	{
		if (!handleInput)
		{
			return;
		}
		currentInteractable = interactable;
		destinationPosition = currentInteractable.interactionLocation.position;

		agent.SetDestination(destinationPosition);
		agent.isStopped = false;
	}

	private IEnumerator WaitforInteraction() //kiedy czekamy na finish interaction to handleinput false
	{
		handleInput = false;

		yield return inputHoldWait;

		while(animator.GetCurrentAnimatorStateInfo(0).tagHash != hashLocomotionTag) // musimy byc w locomotion state 
		{
			yield return null;
		}

		handleInput = true;
	}
}
