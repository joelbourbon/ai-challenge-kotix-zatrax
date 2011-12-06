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
		  var foodComparatorList = new List<FoodComparator>();
		  var myFoodComparatorListSorter = new FoodComparatorListSorter();

      ////
      ////  Loop trough all my ants and foods to list all the matching possibilities
      ////
      if(state.FoodTiles.Count > 0)
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
          foreach (FoodComparator fc in foodComparatorList)
            if (fc.Ant.Equals(wSelectedFoodComparator.Ant) || fc.Food.Equals(wSelectedFoodComparator.Food))
              foodComparatorList.Remove(fc);
        }
      }

		  // loop through all my ants and try to give them orders
			foreach (Ant ant in state.MyAnts) 
      {
				// try all the directions
				foreach (Direction direction in Ants.Aim.Keys) 
        {
					// GetDestination will wrap around the map properly
					// and give us a new location
					Location newLoc = state.GetDestination(ant, direction);

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
            break;
          }

          // GetIsPassable returns true if the location is land
          if (state.GetIsPassable(newLoc) && state.GetIsUnoccupied(newLoc) && !isAlreadyUsed) 
          {
						IssueOrder(ant, direction);
            newLocations.Add(newLoc);
						// stop now, don't give 1 and multiple orders
						break;
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
	}
}