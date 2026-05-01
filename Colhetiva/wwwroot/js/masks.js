document.addEventListener('DOMContentLoaded', function() {
    // Apply masks using Inputmask
    if (typeof Inputmask !== 'undefined') {
        // CPF mask
        var cpfInput = document.getElementById('CPF');
        if (cpfInput) {
            new Inputmask({
                "mask": "999.999.999-99",
                "placeholder": "0",
                "showMaskOnFocus": true,
                "showMaskOnHover": true,
                "repeat": 1,
                "greedy": false
            }).mask(cpfInput);
        }
        
        // CEP mask - limit to 8 digits
        var cepInputs = document.querySelectorAll('input[name="Endereco.Cep"]');
        cepInputs.forEach(function(input) {
            new Inputmask({
                "mask": "99999-999",
                "placeholder": "0",
                "showMaskOnFocus": true,
                "showMaskOnHover": true,
                "repeat": 1,
                "greedy": false
            }).mask(input);
        });
        
        // Telefone mask
        var telefoneInput = document.getElementById('Telefone');
        if (telefoneInput) {
            new Inputmask({
                "mask": "(99) 99999-9999",
                "placeholder": "0",
                "showMaskOnFocus": true,
                "showMaskOnHover": true,
                "repeat": 1,
                "greedy": false
            }).mask(telefoneInput);
        }
    }
    
    // MaxLength attributes for additional validation
    var cepInputs = document.querySelectorAll('input[name="Endereco.Cep"]');
    cepInputs.forEach(function(input) {
        input.setAttribute('maxlength', '9'); // 8 digits + 1 hyphen
    });
    
    var cpfInput = document.getElementById('CPF');
    if (cpfInput) {
        cpfInput.setAttribute('maxlength', '14'); // 11 digits + 2 dots + 1 hyphen
    }
    
    var telefoneInput = document.getElementById('Telefone');
    if (telefoneInput) {
        telefoneInput.setAttribute('maxlength', '15'); // 10 digits + 2 parentheses + 1 space + 1 hyphen
    }
    
    // Clean masks on form submit - more aggressive cleanup
    var forms = document.querySelectorAll('form');
    forms.forEach(function(form) {
        form.addEventListener('submit', function(e) {
            // Clean CPF - keep only digits
            var cpfInput = document.getElementById('CPF');
            if (cpfInput && cpfInput.value) {
                cpfInput.value = cpfInput.value.replace(/\D/g, '');
            }
            
            // Clean CEP - keep only digits
            var cepInputs = form.querySelectorAll('input[name="Endereco.Cep"]');
            cepInputs.forEach(function(input) {
                if (input.value) {
                    input.value = input.value.replace(/\D/g, '');
                }
            });
            
            // Clean Telefone - keep only digits
            var telefoneInput = document.getElementById('Telefone');
            if (telefoneInput && telefoneInput.value) {
                telefoneInput.value = telefoneInput.value.replace(/\D/g, '');
            }
        });
    });
});