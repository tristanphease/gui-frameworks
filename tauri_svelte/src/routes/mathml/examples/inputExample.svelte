<script lang="ts">

    const base = 5;
    let value = $state(2);
    let calculatedValue = $derived(Math.pow(base, value));

</script>

<div>
    <p>Use input values inside of the math contexts</p>
    <math>
        <msup>
            <mn>
                {base}
            </mn>
            <mn>
                <input type=number bind:value={value} />
            </mn>
        </msup>
        <mo>
            =
        </mo>
        <mn>
            {calculatedValue}
        </mn>
    </math>
</div>

<style>
    input {
        background-color: var(--bg-color);
        color: var(--text-color);
        width: 40px;
        font-size: 20px;
    }

    math {
        font-size: 30px;
    }
</style>