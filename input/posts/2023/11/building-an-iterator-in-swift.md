---
Title: Building an iterator in Swift
Slug: building-an-iterator-in-swift
Date: 2023-11-13
Tags:
- Swift
---

<div class="flex mt-4 md:mt-4 mb-4 p-4 border border-primary-400/50  bg-primary-200/50 dark:border-primary-800 dark:bg-primary-700/50 shadow-md rounded-md items-center">
    <p class="ml-4 text-base-700 dark:text-primary-300 text-xs md:text-sm">
        <b>EDIT:</b> Thanks to <a href="https://github.com/idrougge" class="font-normal">Iggy Drougge</a> and 
        <a href="https://github.com/Optiroc" class="font-normal">David Lindecrantz</a> for suggestions and feedback!</p>
</div>

As part of learning Swift, I decided to port my project [Spectre.Console](https://spectreconsole.net/) (written in C#) to [Swift](https://www.swift.org/).

Something extensively used in the project is an extension method that provides information about whether the item being iterated over is the first or last in the collection, as well as its index.

<!--excerpt-->

```csharp
IEnumerable<(int index, bool isFirst, bool isLast, T item)> Enumerate<T>(this IEnumerable<T> source)
```

So, yesterday evening, I sat down to try to port this to Swift and make it as idiomatic as possible (to the extent of my understanding of Swift).

```swift
public struct SequenceIterator<T: IteratorProtocol> : IteratorProtocol, Sequence {
    public typealias Element = (index: Int, isFirst: Bool, isLast: Bool, item: T.Element)
    
    private var iterator: T
    private var current: T.Element?
    private var index = 0
    private var isFirst = true

    public init(iterator: T) {
        self.iterator = iterator
        self.current = self.iterator.next()
    }

    public mutating func next() -> (index: Int, isFirst: Bool, isLast: Bool, item: T.Element)? {
        guard let current else {
            return nil
        }

        let next = iterator.next()
        
        defer {
            self.current = next
            self.isFirst = false
            self.index += 1
        }
        
        return (
            index: self.index, 
            isFirst: self.isFirst, 
            isLast: next == nil, 
            item: current)
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

Now, I can easily iterate over any structure by calling `makeSequenceIterator`:

```swift
let foo = ["A", "B", "C", "D"]

for (index, isFirst, isLast, item) in foo.makeSequenceIterator() {
    print("Index: \(index)")
    print("First: \(isFirst)")
    print("Last: \(isLast)")
    print("Item: \(item)")
    
    if !isLast {
        print("--------------------")
    }
}
```

Since I'm in the process of learning Swift, all tips are very welcome as I aim to learn how to write code as idiomatically as possible.