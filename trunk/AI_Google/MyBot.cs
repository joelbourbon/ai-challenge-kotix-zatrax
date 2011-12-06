using System;
using System.Collections.Generic;

namespace Ants 
{

	class MyBot : Bot 
  {

		// DoTurn is run once per turn
		public override void DoTurn (IGameState state)
		{
		  var newLocations = new List<Location>();

      // Match ant to closer food if:
      //          - Ant doesn't already have a destination
      //          - Ant is a food gatherer
      //          - Food is known (FoodTiles)
      if (state.FoodTiles.Count > 0)
        MatchAntsToFoods(state);

		  // loop through all my ants and try to give them orders
			foreach (Ant ant in state.MyAnts)
			{
        // List of possibilities with the right Logical order
			  var possibilitiesList = new List<Possibilities>();
			  possibilitiesList.Add(Possibilities.Path);
        possibilitiesList.Add(Possibilities.RandomDirection);
        possibilitiesList.Add(Possibilities.RandomDirection);
        possibilitiesList.Add(Possibilities.RandomDirection);
        possibilitiesList.Add(Possibilities.RandomDirection);

			  var directionList = new List<Direction>();
        directionList.Add(Direction.North);
        directionList.Add(Direction.South);
        directionList.Add(Direction.East);
        directionList.Add(Direction.West);

        if (!ant.NoDestination() && ant.Path.Count == 0)
        {
          ant.Path = PathFinding(ant, ant.Destination);
        }

				// try all the directions
        foreach (Possibilities possibility in possibilitiesList)
        {
          bool pathTaken = false;
          int numDirections = 1;
          var nextDirection = new Direction();

          if(ant.Path.Count != 0 && possibility == Possibilities.Path)
          {
            nextDirection = ant.Path[0];
            pathTaken = true;
          }

          else if(possibility == Possibilities.RandomDirection)
          {
            nextDirection = directionList[numDirections];
            numDirections++;
          }

					// GetDestination will wrap around the map properly
					// and give us a new location
          Location newLoc = state.GetDestination(ant, nextDirection);

          // Verify if the wanted position is already planned by another ant
          bool isAlreadyUsed = false;
          foreach (Location loc in newLocations)
            if (newLoc == loc)
              isAlreadyUsed = true;

          // If a food spawn under an ant, do not move for that turn
          foreach(Location foodLoc in state.FoodTiles)
            if(ant.Equals(foodLoc))
              ant.DoNotMove = true;

          if (ant.DoNotMove)
          {
            ant.DoNotMove = false;
            break;  // Break to pass to next ant
          }

          // GetIsPassable returns true if the location is land
          if (state.GetIsPassable(newLoc) && state.GetIsUnoccupied(newLoc) && !isAlreadyUsed) 
          {
            IssueOrder(ant, nextDirection);
            newLocations.Add(newLoc);
            if (pathTaken)
              ant.Path.RemoveAt(0);
            // If at goal, reset goal
            if (ant.Path.Count == 0)
              ant.Destination = new Location(-1, -1);
						break;  // Break to pass to next ant  
					}
				}
				
				// check if we have time left to calculate more orders
				if (state.TimeRemaining < 10) break;
			}
		}
		
		// The main that starts everything
		public static void Main (string[] args)
		{
#if DEBUG
  System.Diagnostics.Debugger.Launch();
  while (!System.Diagnostics.Debugger.IsAttached)
  {}
#endif
      new Ants().PlayGame(new MyBot());
		}

    public class FoodComparator
    {
      public float Distance = 0;
      public Location Food;
      public Location Ant;

      public FoodComparator(float iDistance, Location iAnt, Location iFood)
      {
        Distance = iDistance;
        Food = iFood;
        Ant = iAnt;
      }
    }

    public class FoodComparatorListSorter : IComparer<FoodComparator>
    {
      public int Compare(FoodComparator obj1, FoodComparator obj2) { return obj1.Distance.CompareTo(obj2.Distance); }
    }

    ////
    ////  Loop trough all my ants and foods to list all the matching possibilities
    ////
    public void MatchAntsToFoods(IGameState state)
    {
      var foodComparatorList = new List<FoodComparator>();
      var myFoodComparatorListSorter = new FoodComparatorListSorter();

      foreach (Ant ant in state.MyAnts)
        if (ant.AntGoal == Ant.Goal.Food && ant.NoDestination())  // (Ant)  If goal is food and no destination matched
          foreach (Location foodLocation in state.FoodTiles)      // (Food) For all known food locations
            foodComparatorList.Add(new FoodComparator(state.GetDistance(ant, foodLocation), ant, foodLocation));

      // If the list is not empty, sort it and keep only the best associations
      if (foodComparatorList.Count > 0)
      {
        // Sort the list with custom sorter (sort with distance)
        foodComparatorList.Sort(myFoodComparatorListSorter);

        while (foodComparatorList.Count > 0)
        {
          // Set the best match destination (now at first position in the list) to the respective ant
          state.MyAnts.Find(iAnt => iAnt.Equals(foodComparatorList[0].Ant)).Destination = foodComparatorList[0].Food;

          // Save temporarily the Comparator matched to remove all the repetitive ants & foods from the list
          FoodComparator wSelectedFoodComparator = foodComparatorList[0];
          foodComparatorList.RemoveAll(iComp => iComp.Food.Equals(wSelectedFoodComparator.Food));
          foodComparatorList.RemoveAll(iComp => iComp.Ant.Equals(wSelectedFoodComparator.Ant));
        }
      }
    }

    public List<Direction> PathFinding(Location iStart, Location iGoal)
    {
      List<Direction> DirectionsToGoal = new List<Direction>();
      Location updatedPosition = iStart;

      while (updatedPosition != iGoal)
      {
        if (updatedPosition.Col < iGoal.Col)
        {
          updatedPosition = new Location(updatedPosition.Row, updatedPosition.Col + 1);
          DirectionsToGoal.Add(Direction.East);
        }
        else if (updatedPosition.Col > iGoal.Col)
        {
          updatedPosition = new Location(updatedPosition.Row, updatedPosition.Col - 1);
          DirectionsToGoal.Add(Direction.West);
        }
        else if (updatedPosition.Row < iGoal.Row)
        {
          updatedPosition = new Location(updatedPosition.Row + 1, updatedPosition.Col);
          DirectionsToGoal.Add(Direction.North);
        }
        else if (updatedPosition.Row > iGoal.Row)
        {
          updatedPosition = new Location(updatedPosition.Row - 1, updatedPosition.Col + 1);
          DirectionsToGoal.Add(Direction.South);
        }
      }

      return DirectionsToGoal;
    }

    public enum Possibilities
    {
      Path,
      RandomDirection
    }
  }
}