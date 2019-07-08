using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*  Author: Connor Chamberlain
 *  Date: 21/06/2019
 *  
 *  
 *  
 *  ### SPEC SHEET###
 *  Task: Online Grocery Store  (C#)
 *  Background: 
 *  A concept online grocery store initially based the price of their product on an individual item cost. Previously, if a customer ordered 20 toilet rolls 
 *  then they would be charged 20x the cost of a toilet roll. The store has decided to start selling their products pre-packaged in packs and charging 
 *  the customer on a per pack basis. For example, if the store sells yogurt in packs of 4 and 10 and a customer ordered 14 they would get a pack of 
 *  4 and 10. The store currently sells the following products: 
 *  Name: Sliced Ham    Code: SH3   Packs: 3 @ $2.99,   5 @ $4.49
 *  Name: Yoghurt       Code: YT2   Packs: 4 @ $4.95,   10 @ $9.95,     15 @ $13.95
 *  Name: Toilet Rolls  Code: TR    Packs: 3 @ $2.95,   5 @ $4.45,      9 @ $7.99
 *  
 *  Use the above table to reference the product item details. 
 *  Using the specified coding language, you are required to determine the cost and pack breakdown for each product
 *  to save on shipping space. Each order should contain the minimal number of packs.
 *  Input: Each order has a series of lines with each line containing the number of items followed by the product code. An  example input:
 *  10 SH3
 *  28 YT3
 *  12 TR
 *  Output: A successfully passing test(s) that demonstrates the following output:
 *  10 SH3 $8.98
 *   2 x 5 $4.49
 *  28 YT2 $29.80
 *   2 x 10 $9.95
 *   2 x 4 $4.95
 *  12 TR $10.94
 *   1 x 9 $7.99
 *   1 x 3 $2.95
 *   
 *  Advice: 
 *      The input/outpus format is not important, do whatever feels reasonable.
 *      Include at least 1 test.
 *      Plrease put forward the code that you are happy to be deployed to production system. If something is not clear make an assumption and 
 *      build with those assumptions in mind. Please ensure your assumptions are stored somewhere it can be viewed.
 *  
 * ### END OF SPEC SHEET ###
 * 
 * 
 * 
 * Assumptions:
 *      Program input amount not filtered to exclude amounts where it is impossible to divide evenly without remainders. E.g. only ordering 
 *          one where the smallest pack size is three
 *      Having no remainder takes priority over creating largest packs. E.g. 9 SH3, prefering 3Pack x3, over 5Pack x1, 3Pack x1, remainder 1
 *      
 * Concepts Used: 
 *      Minimum... There is a point where consecutive numbers will always be divisible. Reaching this minimum guarantees optimal division
 *      Maximum... This point is the furthest the consecutive numbers have to meet before redundancy
 *      Specials... These are the divisible cases before the minimum, edge cases that require considering
 *      Hand Shake circle... Circle of 4 people. Everyone must shake hands without doing so twice, how many handshakes are there total?
 *          Person 1: Hand shake with 2, 3, and 4.
 *          Person 2: Hand shake with 3, and 4  having already hand shaked with person one, and cannot handshake oneself.
 *          Person 3: Hand shake with 4.
 *          Person 4: Has already shaken hands with the other people. Therefore he cannot shake hands with anyone else.
 *      
 *      SH3: three and five	(minimum at 8, max at 12), specials = 3, 5, 6
 *      3, 5, 6, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17
 *      
 *      YT2: four, ten, and fifthteen (minimum at 22, max at 36), specials = 4, 8, 10, 12, 14, 15, 16, 18, 19, 20
 *      4, 8, 10, 12, 14, 15, 16, 18, 19, 20, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37
 *      
 *      TR: three, five, nine, (minimum at 8, max at 16), specials = 3, 5, 6
 *      3, 5, 6, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17
 *      
 * VersionTwo: (CURRENTLY SELECTED)
 *      This solution has improved over V1 in its organisation and reduction of code. This version also automates some of the processes used
 *          in V1.
 *      Adding products still requires some setup. A potential error is the order for which pack sizes are added to the list,
 *          e.g. (4, 10, 15) rather than (15, 10, 4)
 *      Further optimisation could be achieved by saving product data to an external file. Currently the initial setup of the application
 *          is expensive, O(4n) in some spots.
 *      This version handles the current data well, however there will be issues if the pack sizes dont work with the minimum maximum 
 *          approach. Pack sizes which are divisible, e.g. (3 & 9) will always have the larger pack take priority. 
 *      User prompts have been added to make the program user friendly and filter input that could cause issues.
 *      Summaries to classes and methods will need to be added, comments however are still provided.
 *      Untested for pack size alternatives greater than three.
 *      If there are no packs then singles will have to be specified as a pack.
 *      It may be possible for divisible packs to favour smaller packs over bigger packs in CreateDivisibles(). Further testing required.
 *      It would be great to think of better ways to rename some variables.
 */

namespace OnlineGroceryStore
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Product library setup
            //Initial library which all item codes (KEY) are checked against, the value is the product data used for pack sizing.
            Dictionary<string, VersionTwo.ProductBase> itemLibrary = new Dictionary<string, VersionTwo.ProductBase>();
            
            VersionTwo.ProductBase temp = new VersionTwo.SlicedHam();   //Temporary object for initialising sliced ham.
            itemLibrary.Add(temp.itemCode, temp);

            temp = new VersionTwo.Yoghurt();                            //Object redeclared from sliced ham (above) to yoghurt.
            itemLibrary.Add(temp.itemCode, temp);

            temp = new VersionTwo.ToiletRolls();                        
            itemLibrary.Add(temp.itemCode, temp);
            #endregion
            
            string input = "";                      //Storage for user input.
            bool error = false;                     //Used to control input handling on user error.

            Console.WriteLine("Please input a quantity and item code e.g. \"10 YT2\" (Type EXIT to exit):");
            input = Console.ReadLine();
            input += " ";

            //Keeps the user in the program for package sizing.
            while (!input.Contains("EXIT"))
            {
                error = false;                          //Reset for new user inputs.
                int i = 0;                              //User input iterator, stores the handlers current point in the input string variable.
                int amount = 0;                         //Integer quantity (inputted from user), used in calculation of packs.
                string amountString = "";               //String variable of above amount vairable. Initial storage of user input.
                string itemCode = "";                   //Storage of user inputted item code.
                char currentCharacter = ' ';            //current character in user string (int i variable).

                //Safely check if the user entered anything.
                try
                {
                    currentCharacter = input[i];
                }
                catch (IndexOutOfRangeException)
                {
                    error = true;
                    Console.WriteLine("Please type a quantity and item code e.g. \"10 YT2\" (Type EXIT to exit)");
                    break;
                }

                #region Quantity Handling
                //Processing the user entered string to obtain the quantity.
                if (error == false)
                {
                    //Loop through individual characters until the entire quantity is processed.
                    do
                    {
                        currentCharacter = input[i];
                        amountString += currentCharacter;
                        i++;

                        //Safely gets next character.
                        try
                        {
                            currentCharacter = input[i];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            error = true;
                            break;
                        }
                    }
                    while (Char.IsDigit(input[i]));

                    //Safely attempts to turn the user entered string into an integer.
                    try
                    {
                        amount = Int32.Parse(amountString);
                    }
                    catch (FormatException)
                    {
                        error = true;
                    }

                    if (amount <= 0)
                    {
                        error = true;
                        Console.WriteLine("Please enter a value greater than zero.");
                    }
                }

                //Check to see if the user entered an item code before a quantity (or just item code).
                if (char.IsLetter(currentCharacter) && error == false)
                {
                    error = true;
                    Console.WriteLine("Please type a quantity before an item code.");
                }

                //To skip spaces and prepare for item code handling
                if (error == false)
                {
                    while (currentCharacter == ' ')
                    {
                        i++;
                        try
                        {
                            currentCharacter = input[i];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Console.WriteLine("Please type an item code after the quantity.");
                            error = true;
                            break;
                        }
                    }
                }
                #endregion

                #region Item Code Handling
                //Obtain the item code.
                if (error == false)
                {
                    //Iterate through to obtain entire item code.
                    while (currentCharacter != ' ')
                    {
                        itemCode += currentCharacter;
                        i++;

                        //Safely iterates through user input.
                        try
                        {
                            currentCharacter = input[i];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            break;
                        }
                    }
                }
                #endregion

                #region Pack Size Output
                //Attempts to output pack size for the inputed item code and quantity.
                if (error == false)
                { 
                    //If the item code exists in the item library, find optimal packaging.
                    if (itemLibrary.ContainsKey(itemCode))
                    {
                        Console.WriteLine("");
                        itemLibrary[itemCode].FindOptimalPackaging(amount);
                    }
                    //If the code doesn't exist report an error.
                    else
                    {
                        error = true;
                        Console.WriteLine("Item code not recognised. Please try again.");
                    }
                }
                #endregion

                //Prepare for next user input.
                Console.WriteLine("");
                Console.WriteLine("Please input a quantity and item code e.g. \"10 YT2\" (Type EXIT to exit):");
                input = Console.ReadLine();
                input += " ";
            }
            System.Environment.Exit(0);
        }

        public class VersionTwo
        {
            //Contains data for calculation of quantity divisibility.
            //NOTE: HAS TWO WAYS OF USE, in packaging optimisation.
            public struct PackContainer
            {
                public int packSize;        //Typically the size of the pack.
                public int quantity;        //Quantity used for this particular pack size (internal data storage for external calculation).
            }

            public class ProductBase
            {
                public List<int> packSizes;     //DECREASING ORDER, list of all pack sizes.
                public List<Double> pricing;    //DECREASING ORDER, POSITION MATCHES PACKSIZES POSITION ABOVE, list of corresponding prices.
                public string itemCode;         //The item code for this product.
                protected int minimum;          //The minimum approach (mentioned at top of script), used to mark starting point of consecutive divisible amounts.
                protected int maximum;          //The maximum approach (mentioned at top of script), used with minimum variable, avoids redundancy covering infinite amounts.
                public Dictionary<int, List<PackContainer>> divisiblePacks;     //List of all possible pack divisibles between 0 and maximum.

                //Setup function to create the divisible amounts for storage and quick access. (Dictionary divisiblePacks).
                //Processes all pack sizes with preference being for larger pack sizes allocated first.
                /*Pseudocode version:
                 * Obtain single pack multiples, e.g. (15), 15, 30, 45.
                 * Add these to divisiblePacks, and save a seperate copy for later convenience in step 2 and 4.
                 * Obtain combination packs between two different packs (combinations). e.g. (15, 3), 15+3, 15 + 3 + 3.
                 * Find values between minimum and maximum that have been missed (as this section is meant to be covered consecutively).
                 * Using the conveniently saved singles pack multiples, subtract this value from the missing values and check the result against the divisiblePacks library.
                 *      This will take advantage of the combo packs in step 2. (15, 10, 4) e.g. 31 - 15 = 16.   16 is in the list as 4 x 4.
                 */
                public void CreateDivisibles()
                {
                    //Temporary storage of divisibles between 0 and max using only one pack size.
                    Dictionary<int, List<PackContainer>> temporarySingleDivisibles = new Dictionary<int, List<PackContainer>>();
                    HashSet<int> alreadyContained = new HashSet<int>();     //Quick storage of already covered values.


                    //1st: Get list of all single pack multiples between 0 and max; Stop upon value match or out of bounds.
                    //Iterates through all pack sizes and stores seperately from divisiblePacks.
                    for (int i = 0; i < packSizes.Count; i++)
                    {
                        int packSize = packSizes[i];                                //For convenient access.
                        List<PackContainer> results = new List<PackContainer>();    //Imitates divisiblePacks however ONLY FOR SINGLE COMBINATIONS, seperate storage.
                        int iteration = 1;                                          //Current iteration (used to store quantity)
                        int current = packSize;                                     //Current quantity * pack size.

                        //Multiply pack size within bounds.
                        while (current < maximum)
                        {
                            //Ignore duplicate entries.
                            if (alreadyContained.Contains(current))
                            {
                                break;
                            }

                            PackContainer pack = new PackContainer();   //Temporary storage, AS MENTIONED IN DECLARATION, SECONDARY USE OF STRUCT. See List<PackContainer> results
                            pack.packSize = current;                    //Saves the current divisible total (similar to Dictionary key of divisible packs).
                            pack.quantity = iteration;                  //Saves the quantity for this particular total.
                            results.Add(pack);                          
                            alreadyContained.Add(current);              //This total is covered, remember to prevent later duplicates.
                            iteration++;
                            current += packSize;
                        }
                        temporarySingleDivisibles.Add(packSize, results);   //Save the divisibles between the bounds for this single pack size.
                    }
                    //For each pack possible pack size, add to divisiblePacks to save temporary storage single divisibles (only needed later).
                    foreach (int packSize in packSizes)
                    {
                        List<PackContainer> packSizeMultiples = temporarySingleDivisibles[packSize];
                        //Iterate through each pack multiple, eg pack size of 3:     3, 6, 9, 12.
                        foreach (PackContainer packMultiple in packSizeMultiples)
                        {
                            List<PackContainer> toAdd = new List<PackContainer>();      //List of only one pack container (as only one pack size is used).
                            PackContainer temp = new PackContainer();
                            temp.packSize = packSize;                                   //Proper use of pack container, pack size is now size, not total including quantity.
                            temp.quantity = packMultiple.quantity;                      //Quantity of pack size to reach total.
                            toAdd.Add(temp);
                            divisiblePacks.Add(packMultiple.packSize, toAdd);           //Add new divisible pack with actual divisible number and associated pack size and quantity.
                        }
                    }


                    //2nd: Get list of combinations between two packs, max height is count. 
                    //First pack combo. Should not hit very last pack size. Hand shake circle problem (mentioned at top of script).
                    for (int i = 0; i < packSizes.Count - 1; i++)
                    {
                        List<PackContainer> outerTemp = temporarySingleDivisibles[packSizes[i]];        //Outer pack size.
                        //Second pack to make up two pack combo. Allowed to hit final pack.
                        for (int x = i + 1; x < packSizes.Count; x++)
                        {
                            //here we can add directly to the divisible packs. No use for temporary storage.
                            List<PackContainer> innerTemp = temporarySingleDivisibles[packSizes[x]];    //Inner pack size.
                            //Iterate through first pack combo's second.
                            foreach (PackContainer outer in outerTemp)
                            {
                                //Cover all possible combo's with pack one (iterating through pack twos combo's).
                                foreach (PackContainer inner in innerTemp)
                                {
                                    int result = outer.packSize + inner.packSize;   //The combination total.
                                    //Within bounds.
                                    if (result > maximum)
                                    {
                                        break;
                                    }
                                    //If this total has not yet been covered.
                                    if (!alreadyContained.Contains(result))
                                    {
                                        List<PackContainer> toAdd = new List<PackContainer>();          //Pack combination to be added, more than one pack size.
                                        PackContainer temp = new PackContainer();                       //Temp storage for pack, data changed with each pack size.
                                        //Pack size 1.
                                        temp.packSize = packSizes[i]; temp.quantity = outer.quantity;
                                        toAdd.Add(temp);
                                        //Pack size 2.
                                        temp.packSize = packSizes[x]; temp.quantity = inner.quantity;
                                        toAdd.Add(temp);
                                        //Save total to memory (to prevent duplicates) and add to divisible packs.
                                        alreadyContained.Add(result);
                                        divisiblePacks.Add(result, toAdd);
                                    }
                                }
                            }
                        }
                    }


                    //3rd: Find any values between the minimum and maximum that still require calculating.
                    List<int> missingValues = new List<int>();      //Missing values to be covered between min and max.
                    for (int i = minimum; i <= maximum; i++)        //Between bounds, find missing values and add to list.
                    {
                        if (!alreadyContained.Contains(i))
                        {
                            missingValues.Add(i);
                        }
                    }


                    //4th: Traverse missing values. Subtract values from the temp singles list in decreasing order, then compare result to already divisible packs.
                    foreach (int missingValue in missingValues)
                    {
                        //Traverse pack sizes.
                        for (int i = 0; i < packSizes.Count; i++)
                        {
                            List<PackContainer> outerTemp = temporarySingleDivisibles[packSizes[i]];    //Convenient access to single pack size combinations.
                            //For each total combination in that pack.
                            foreach (PackContainer pack in outerTemp)
                            {
                                int result = missingValue - pack.packSize;      //New result to find from divisibles list.
                                //Success, found number in divisibles list.
                                if (divisiblePacks.ContainsKey(result))
                                {
                                    //Save initially subtracted pack size and quantity.
                                    PackContainer outerContainerTemp = new PackContainer();                                 
                                    outerContainerTemp.packSize = packSizes[i]; outerContainerTemp.quantity = pack.quantity;
                                    
                                    //Creates a temporary list for what to add. Saves the packs and quantities for the new value.
                                    List<PackContainer> toAdd = new List<PackContainer>();
                                    List<PackContainer> FromBase = divisiblePacks[result];

                                    //Combine initially subtracted pack and new pack division. Preference to keep in sorted decreasing order.
                                    foreach (PackContainer toCheck in FromBase)
                                    {
                                        //If the outer pack has a higher value, add before the other packs.
                                        if (outerContainerTemp.packSize > toCheck.packSize)
                                        {
                                            toAdd.Add(outerContainerTemp);
                                        }
                                        toAdd.Add(toCheck);
                                    }

                                    //Save the new total and pack size + quantities list to of divisiblePacks.
                                    divisiblePacks.Add(missingValue, toAdd);
                                    alreadyContained.Add(missingValue);
                                    break;  //Escape this missing value.
                                }
                            }
                            //From above break, with value already added, escape this missing value.
                            if (alreadyContained.Contains(missingValue))
                            {
                                break;  //Escape this missing value.
                            }
                        }
                    }
                }

                //Given an amount find and print the pack sizes and quantities which make up that amount.
                public void FindOptimalPackaging(int Amount)
                {
                    Dictionary<int, int> packData = new Dictionary<int, int>();     //Local storage for packsizes and quantities.
                    //Initialise with all pack sizes and zero quantity.
                    for (int i = packSizes.Count - 1; i >= 0; i--)
                    {
                        packData.Add(packSizes[i], 0);
                    }
                    
                    int remainder = Amount;     //Local changeable storage of Amount.

                    //1st: Divide by largest amount, as much as possible.
                    int divisions = remainder / packSizes[0];
                    packData[packSizes[0]] += divisions;
                    remainder -= divisions * packSizes[0];


                    //2nd: Check divisible packs, otherwise undo largest pack size until divisible.
                    //Already divisible.
                    if (divisiblePacks.ContainsKey(remainder))
                    {
                        packData = AddDivisiblePacks(packData, remainder);
                        remainder = 0;
                    }
                    //So long as remainder is less than minimum (should already be after above check).
                    if (remainder < minimum)
                    {
                        //Take off the largest pack (while there is at least one and less than minimum) and re-add the pack size amount to the remainder.
                        while (packData[packSizes[0]] > 0 && remainder < minimum)
                        {
                            packData[packSizes[0]]--;
                            remainder += packSizes[0];
                        }
                        //Attempt to find this new remainder within the list of divisiblePacks.
                        if (divisiblePacks.ContainsKey(remainder))
                        {
                            packData = AddDivisiblePacks(packData, remainder);
                            remainder = 0;
                        }
                    }


                    //3rd: Handle any remainders that arent handled by the optimisation function.
                    int reductions = 0;
                    //Incrementally take from reductions and comparet the new value against the divisible packs.
                    while(remainder > 0)
                    {
                        reductions++;
                        remainder--;
                        //If the new figure is in the divisible packs, then calculate pack quantities.
                        if (divisiblePacks.ContainsKey(remainder))
                        {
                            packData = AddDivisiblePacks(packData, remainder);
                            remainder = 0;
                        }
                    }
                    //Re-add the reductions to the singles items left over.
                    remainder += reductions;


                    //4th: Display results to the user.
                    double price = 0;       //Total price figure.
                    //Iterate through pack sizes and get the associated quantity for the specified amount. Calculate the total dollar figure.
                    for (int i = 0; i < packSizes.Count; i++)
                    {
                        //Calculate total price.
                        price += ((double)packData[packSizes[i]]) * pricing[i];
                    }

                    //Display quantity, item code, and total price.
                    Console.WriteLine(Amount + " " + itemCode + " $" + GetPriceString(price));

                    //For each pack size, display the quantity which makes it up and its price (not considering quantity).
                    for (int i = 0; i < packSizes.Count; i++)
                    {
                        //So long as the quantity is greater than zero.
                        if (packData[packSizes[i]] != 0)
                        {
                            Console.WriteLine("  " + packData[packSizes[i]] + " x " + packSizes[i] + " $" + GetPriceString(pricing[i]));
                        }
                    }
                    //Display any remaining figures from the quantity.
                    if (remainder > 0)
                    {
                        Console.WriteLine("  " + remainder + " x " + "1");
                    }
                }

                //Adds the pack size and quantity for the associated amount.
                private Dictionary<int, int> AddDivisiblePacks(Dictionary<int, int> Current, int Key)
                {
                    List<PackContainer> temp = divisiblePacks[Key];     //Copy of the pack size and quantities for that particular amount.
                    //Iterate through and add the amount to the corresponding pack size.
                    foreach (PackContainer item in temp)
                    {
                        Current[item.packSize] += item.quantity;
                    }
                    return Current;
                }

                //Turns (double) prices into strings and formatting correctly.
                private string GetPriceString(double Price)
                {
                    string result = Price.ToString();
                    //If the amount is has a decimal component
                    if (result.Length - 2 > 0)
                    {
                        //if the decimal part is only a length of one (e.g. $12.9), add a zero ($12.90).
                        if (result[(result.Length - 2)] == '.')
                        {
                            result += "0";
                        }
                    }
                    return result;
                }

            }

            public class SlicedHam : ProductBase
            {
                public SlicedHam()
                {
                    packSizes = new List<int>();
                    pricing = new List<double>();
                    packSizes.Add(5);
                    pricing.Add(4.49);
                    packSizes.Add(3);
                    pricing.Add(2.99);
                    itemCode = "SH3";
                    minimum = 8;
                    maximum = 12;
                    divisiblePacks = new Dictionary<int, List<PackContainer>>();
                    CreateDivisibles();
                }
            }
            public class Yoghurt : ProductBase
            {
                public Yoghurt()
                {
                    packSizes = new List<int>();
                    pricing = new List<double>();
                    packSizes.Add(15);
                    pricing.Add(13.95);
                    packSizes.Add(10);
                    pricing.Add(9.95);
                    packSizes.Add(4);
                    pricing.Add(4.95);
                    itemCode = "YT2";
                    minimum = 22;
                    maximum = 36;
                    divisiblePacks = new Dictionary<int, List<PackContainer>>();
                    CreateDivisibles();
                }
            }
            public class ToiletRolls : ProductBase
            {
                public ToiletRolls()
                {
                    packSizes = new List<int>();
                    pricing = new List<double>();
                    packSizes.Add(9);
                    pricing.Add(7.99);
                    packSizes.Add(5);
                    pricing.Add(4.45);
                    packSizes.Add(3);
                    pricing.Add(2.95);
                    itemCode = "TR";
                    minimum = 8;
                    maximum = 16;
                    divisiblePacks = new Dictionary<int, List<PackContainer>>();
                    CreateDivisibles();
                }
            }
        }
    }
}
