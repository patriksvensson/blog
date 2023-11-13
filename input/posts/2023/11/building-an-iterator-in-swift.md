---
Title: Building an iterator in Swift
Slug: building-an-iterator-in-swift
Date: 2023-11-13
Tags:
- Swift
---

As part of learning Swift, I decided to port my project [Spectre.Console](https://spectreconsole.net/) (written in C#) to [Swift](https://www.swift.org/).

Something extensively used in the project is an extension method that provides information about whether the item being iterated over is the first or last in the collection, as well as its index.

<!--excerpt-->

```csharp
IEnumerable<(int index, bool first, bool last, T item)> Enumerate<T>(this IEnumerable<T> source);
```

So, yesterday evening, I sat down to try to port this to Swift and make it as idiomatic as possible (to the extent of my understanding of Swift).

```swift
public struct SequenceIterator<T: IteratorProtocol> : IteratorProtocol, Sequence {
    public typealias Element = (index: Int, first: Bool, last: Bool,  item: T.Element)
    
    var items: T
    var current: T.Element?
    var index: Int
    var first: Bool

    public init(iterator: T) {
        self.items = iterator
        self.current = self.items.next()
        self.index = 0
        self.first = true
    }

    public mutating func next() -> (index: Int, first: Bool, last: Bool,  item: T.Element)? {
        // Nothing more to yield?
        guard self.current != nil else {
            return Optional.none
        }
        
        let current = self.current.unsafelyUnwrapped
        let next = self.items.next()
        
        let valueToReturn =
            (index: self.index, first: self.first, last: next == nil, item: current)

        self.index += 1
        self.current = next
        self.first = false;

        return Optional.some(valueToReturn)
    }
}
```

I know, I know, `SequenceIterator` is not the best name, but naming is, after all, one of the hardest problems in computer science.

To make `SequenceIterator` a bit easier to use, I also created an extension for `Sequence`.

```swift
extension Sequence {
    public func makeSequenceIterator() -> SequenceIterator<Self.Iterator> {
        return .init(iterator: makeIterator())
    }
}
```

Now, I can easily iterate over any structure by calling makeSequenceIterator:

```swift
let foo = ["A", "B", "C", "D"]

for (index, first, last, item) in foo.makeSequenceIterator() {
    print("Index: \(index)")
    print("First: \(first)")
    print("Last: \(last)")
    print("Item: \(item)")
    
    if !last {
        print("--------------------")
    }
}
```

Since I'm in the process of learning Swift, all tips are very welcome as I aim to learn how to write code as idiomatically as possible.