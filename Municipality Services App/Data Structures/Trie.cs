using System.Collections.Generic;

namespace Municipality_Services_App
{
    /// <summary>
    /// chatGPT recomended i use a Trie data structure for the location autocomplete
    /// and it did help with implementation but all comments explaining ot works
    /// are typed out by hand to show that i actually understand whats going on.
    /// </summary>
    public class TrieNode
    {
        /// <summary>
        /// so each node stores a dictionary containing all the immediate children
        /// it stores the next character in the chain and that characters Node. then that node can be used to navigate further down the tree.
        /// </summary>
        public Dictionary<char, TrieNode> Children { get; } = new Dictionary<char, TrieNode>();

        /// <summary>
        /// used to check that this is the lowest point in the tree to prevent out of bounds exception. the full word string is also populated only at the 
        /// lowest node on a branch. so if isEndOfWord == true then FullString will have the full string.
        /// </summary>
        public bool IsEndOfWord { get; set; }

        /// <summary>
        /// the full string the branch is storing
        /// </summary>
        public string FullString { get; set; }
    }

    public class Trie
    {
        //the root stores a dictionary of all the possible starting characters
        private TrieNode root = new TrieNode();

        public void Insert(string newString)
        {
            var node = root;
            // for each char in the new string (first convert all of them to lowercaase
            foreach (var c in newString.ToLower())
            {
                //dictionarys cant contain duplicates which works in our favour here because we want
                //to attach this new word to a pre-existing branch if one exists
                if (!node.Children.ContainsKey(c))
                {
                    // if there is no pre-existing entry with c then create one
                    node.Children[c] = new TrieNode();
                }
                // if a branch with c already exists move to that node to repeat the proccess
                node = node.Children[c];
            }
            //once  you get to the last node attach the full string
            // this part was implemented by chatGPT but i dont like how space inefficient it is. might remove it
            // and take the performace hit over the extra storage
            node.IsEndOfWord = true;
            node.FullString = newString;
        }

        /// <summary>
        /// gets the lowest node in the tree that a prefix navigates to and collects all the branches that stem from that node.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="maxSuggestions"></param>
        /// <returns></returns>
        public List<string> GetSuggestions(string prefix, int maxSuggestions = 5)
        {
            var node = root;
            foreach (var c in prefix.ToLower())
            {
                // this goes to the lowest node that the prefix can access. so if the prefix is "la" then the node it will end up on
                // might contain paths for "mp" "st" "augh" "te" etc. this is the cool part of using a Trie. it makes a search like this super easy.
                if (!node.Children.ContainsKey(c))
                {
                    return new List<string>();
                }
                node = node.Children[c];
            }

            //begin recursivley adding suggestions
            var suggestions = new List<string>();
            CollectSuggestions(node, suggestions, maxSuggestions);
            return suggestions;
        }

        /// <summary>
        /// recursivley look through each node layer to add all the branches to a list
        /// </summary>
        /// <param name="node"></param>
        /// <param name="suggestions"></param>
        /// <param name="maxSuggestions"></param>
        private void CollectSuggestions(TrieNode node, List<string> suggestions, int maxSuggestions)
        {
            // this is the base case. a node might already be at the end of a word so just add the fullstring to the suggestions
            if (node.IsEndOfWord)
            {
                suggestions.Add(node.FullString);
            }
            // otherwise go through each child node of the current node. 
            foreach (var child in node.Children.Values)
            {
                //assuming we havent reached the max number of suggestions
                if (suggestions.Count >= maxSuggestions)
                {
                    break;
                }
                // recursive call. because we can trust the base case!!!! we can do the same thing over and over until the base case is met!!
                CollectSuggestions(child, suggestions, maxSuggestions);
            }
        }
    }
}
//----------------------------------------------------End_of_File----------------------------------------------------//