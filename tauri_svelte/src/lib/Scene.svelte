<script lang="ts">
    import { T } from "@threlte/core";
    import ScrollCamera from "./scrollCamera.svelte";
    import { Text3DGeometry } from "@threlte/extras";
    import Page from "../routes/+page.svelte";

    let scrollY = $state(0);

    const CHARS = "0123456789+-π=";

    const OBJECT_NUM = 30;

    type Position = [number, number, number];
    interface TextObject {
        position: Position,
        character: string,
    }

    interface TextObjectInfo {
        initialPos: Position,
        gradient: number,
        character: string,
    }

    let charObjectInfos: TextObjectInfo[] = $state([]);
    let charObjects: TextObject[] = $derived(calcCharObjectFromInfo(charObjectInfos, scrollY));

    function calcCharObjectFromInfo(charObjectInfos: TextObjectInfo[], scrollY: number): TextObject[] {
        return charObjectInfos.map(x => {

            let width = document.documentElement.clientWidth;
            let height = document.documentElement.clientHeight;

            let newPosition: Position = [add(x.initialPos[0], scrollY, width, x.gradient), add(x.initialPos[1], scrollY, height, 1 - x.gradient), x.initialPos[2]];
            
            return {
                position: newPosition,
                character: x.character,
            };

            function add(pos: number, scroll: number, max: number, gradient: number): number {
                return (pos + scroll * gradient + max/2) % max - max/2;
            }
        });
    }

    function addObject() {
        const randomChar = CHARS[Math.floor(Math.random() * CHARS.length)];

        let width = document.documentElement.clientWidth;
        let height = document.documentElement.clientHeight;

        // simple triangle distribution between -value to value
        const initialPos: Position = [randBias(width), randBias(height), 200 * (Math.random() - 0.5)];

        charObjectInfos.push({
            initialPos,
            gradient: Math.random() * 2 - 1,
            character: randomChar
        });
    }

    // bias the numbers towards the centre
    function randBias(value: number) {
        const random = Math.random();
        const sign = Math.random() < 0.5 ? 1 : -1;
        const randomSquared = (1 - random * random) * sign;
        return randomSquared * value;
    }

    // initial setup
    for (let i = 0; i < OBJECT_NUM; i++) {
        addObject();
    }

</script>

<svelte:window
  bind:scrollY={scrollY}
/>

<ScrollCamera />

<T.AmbientLight 
    intensity={2.0}
/>


{#each charObjects as charObject}
    <T.Mesh
        position={charObject.position}
    >
        <Text3DGeometry text={charObject.character} />
        <T.MeshStandardMaterial color="black" emissive="white" />
    </T.Mesh>
{/each}
