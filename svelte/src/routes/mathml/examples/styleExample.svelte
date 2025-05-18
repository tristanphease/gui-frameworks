<div>
    <p>Can style maths related elements using css</p>
    <math>
        <mi>x</mi>
        <mo>=</mo>
        <mfrac>
            <mrow>
                <mi>-b</mi> 
                <mo>&PlusMinus;</mo> 
                <msqrt>
                    <mrow><msup><mi>b</mi><mn>2</mn></msup><mo>-</mo><mn>4</mn><mi>ac</mi></mrow>
                </msqrt>
            </mrow>
            <mrow><mn>2</mn><mi>a</mi></mrow>
        </mfrac>
    </math>
</div>

<style>
    @property --rotation {
        syntax: "<number>";
        initial-value: 0;
        inherits: false;
    }

    math {
        font-size: 30px;

        color: hsl(var(--rotation) 70% 60%);
        animation: 1s font-expand infinite alternate linear, 5s rotate-angle infinite linear;
    }

    @keyframes font-expand {
        from {
            font-size: 30px;
        }

        to {
            font-size: 32px;
        }
    }

    

    @keyframes rotate-angle {
        from {
            --rotation: 0;
        }
        to {
            --rotation: 360;
        }
    }

</style>