<script>
    import { Canvas } from "@threlte/core";
    import Scene from "./Scene.svelte";


</script>



<div>
    <div id="canvasWrapper">
        <Canvas>
            <Scene />
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
        width: 100vw;
        height: 100vh;
    }

    :global(body) {
        margin: 0;
    }
</style>
