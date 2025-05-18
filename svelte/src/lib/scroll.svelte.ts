/**
 * A function that takes the distance through, between 0 and 1
 */
type TransformFunction = (amount: number) => void;

export function scrollBehaviour(
    node: HTMLElement,
    [transformFunction, height]: [TransformFunction, number],
) {
    

    const dummyElem = document.createElement("div");
    dummyElem.style.height = `${height}px`;
    dummyElem.style.visibility = "hidden";
    node.insertAdjacentElement("afterend", dummyElem);
    node.style.position = "absolute";
    node.style.left = "0px";
    node.style.top = "0px";

    let scrollY = $state(0);
    const dummyElemBox = dummyElem.getBoundingClientRect();
    const nodeBox = node.getBoundingClientRect();
    const windowHeight = window.document.documentElement.clientHeight;
    let clampedScrollYElem = $derived(Math.min(Math.max((scrollY - dummyElemBox.top) / (dummyElemBox.height - nodeBox.height), 0), 1));

    function run() {
        scrollY = window.scrollY;
        
        // node.style.top = `${}px`;

        // if bottom of window is less than top of elem or
        // top of window is greater than bottom of elem
        // then just hide elem
        if (scrollY + windowHeight <= dummyElemBox.top || scrollY >= dummyElemBox.bottom) {
            node.style.visibility = "hidden";
        } else {
            // if top of window is less than top of elem
            // display partially by the difference
            if (scrollY <= dummyElemBox.top) {
                node.style.top = `${dummyElemBox.top}px`;
            // if top of window is greater than top of elem
            } else if (scrollY >= dummyElemBox.bottom - nodeBox.height) {
                node.style.top = `${dummyElemBox.bottom - nodeBox.height}px`;
            } else {
                node.style.top = `${scrollY}px`;
            }
            
            node.style.visibility = "visible";
        }
        transformFunction(clampedScrollYElem);
    }

    run();

    window.addEventListener("scroll", run);
}
