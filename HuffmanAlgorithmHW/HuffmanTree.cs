using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanAlgorithmHW
{
    public class HuffmanTree
    {
        private List<Node> nodes = new List<Node>();
        public Node Root { get; set; }
        public Dictionary<char, int> Frequencies = new Dictionary<char, int>();


        public void Build(string source)
        {
            // Karakterleri tara ve frekansı bul
            for (int i = 0; i < source.Length; i++)
            {
                if (!Frequencies.ContainsKey(source[i]))
                {
                    Frequencies.Add(source[i], 0);
                }

                Frequencies[source[i]]++;
            }

            // Bulunan karakterleri nodes listesine ekle
            foreach (KeyValuePair<char, int> symbol in Frequencies)
            {
                nodes.Add(new Node() { Symbol = symbol.Key, Frequency = symbol.Value });
            }


            // Burada nodes listesini kullanarak huffman ağacı oluştur
            while (nodes.Count > 1)
            {
                List<Node> orderedNodes = nodes.OrderBy(node => node.Frequency).ToList<Node>();

                // Burada sıralanan orderedNodes listesinden, ilk iki elemanı alıp parentNode oluşturduk ve kullandığımız iki elemanı orderedNodes listesinden çıkardık
                // parentNode için * karakteri kullanıldı
                if (orderedNodes.Count >= 2)
                {
                    // ilk iki elemanı al
                    List<Node> taken = orderedNodes.Take(2).ToList<Node>();

                    // parentNode oluşturduk
                    Node parent = new Node()
                    {
                        Symbol = '*',
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1]
                    };
                    // parentnode için kullandığımız iki elemanı orderedNodes listesinden çıkardık
                    // parentnode u ekledik
                    nodes.Remove(taken[0]);
                    nodes.Remove(taken[1]);
                    nodes.Add(parent);
                }

                // Root değerimizi her seferinde güncelleyerek root değerine ulaştık
                this.Root = nodes.FirstOrDefault();

            }

        }

        // huffman ile sıkıştırılacak karakterler veriyoruz ve bize bit listesi dönüyor
        public BitArray Encode(string source)
        {
            List<bool> encodedSource = new List<bool>();
            // tüm karakterleri tara ve encode et
            for (int i = 0; i < source.Length; i++)
            {
                List<bool> encodedSymbol = this.Root.Traverse(source[i], new List<bool>());
                encodedSource.AddRange(encodedSymbol);
            }

            BitArray bits = new BitArray(encodedSource.ToArray());

            return bits;
        }

        // huffman ile encode edilen bitleri karaktere çeviriyoruz
        public string Decode(BitArray bits)
        {
            Node current = this.Root;
            string decoded = "";

            foreach (bool bit in bits)
            {
                if (bit)
                {
                    if (current.Right != null)
                    {
                        current = current.Right;
                    }
                }
                else
                {
                    if (current.Left != null)
                    {
                        current = current.Left;
                    }
                }

                if (IsLeaf(current))
                {
                    decoded += current.Symbol;
                    current = this.Root;
                }
            }

            return decoded;
        }

        // verilen node'un alt sağ ve sol node'u yoksa leaf'tır dedik
        public bool IsLeaf(Node node)
        {
            return (node.Left == null && node.Right == null);
        }




    }
}
