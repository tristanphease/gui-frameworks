<script>
    import { Canvas } from "@threlte/core";
    import Scene from "./Scene.svelte";

    let scrollY = $state(0);

    function onScroll() {
        scrollY = Math.max(window.scrollY, 0);
    }

</script>

<svelte:window
  on:scroll={onScroll}
/>

<div>
    <div id="canvasWrapper">
        <Canvas>
            <Scene scrollY={scrollY} />
        </Canvas>
    </div>
    <div id="dummy">

    </div>
</div>

<style>
    #dummy {
        height: 2000px;
        visibility: hidden;
    }

    #canvasWrapper {
        position: sticky;
        top: 0;
        width: 90vw;
        height: 90vh;
    }
</style>
