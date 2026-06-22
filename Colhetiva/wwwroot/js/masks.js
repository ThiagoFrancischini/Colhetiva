document.addEventListener('DOMContentLoaded', function() {
    if (typeof Inputmask !== 'undefined') {
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
    
    var cepInputs = document.querySelectorAll('input[name="Endereco.Cep"]');
    cepInputs.forEach(function(input) {
        input.setAttribute('maxlength', '9');
    });
    
    var cpfInput = document.getElementById('CPF');
    if (cpfInput) {
        cpfInput.setAttribute('maxlength', '14');
    }
    
    var telefoneInput = document.getElementById('Telefone');
    if (telefoneInput) {
        telefoneInput.setAttribute('maxlength', '15');
    }
    
    var forms = document.querySelectorAll('form');
    forms.forEach(function(form) {
        form.addEventListener('submit', function(e) {
            var cpfInput = document.getElementById('CPF');
            if (cpfInput && cpfInput.value) {
                cpfInput.value = cpfInput.value.replace(/\D/g, '');
            }
            
            var cepInputs = form.querySelectorAll('input[name="Endereco.Cep"]');
            cepInputs.forEach(function(input) {
                if (input.value) {
                    input.value = input.value.replace(/\D/g, '');
                }
            });
            
            var telefoneInput = document.getElementById('Telefone');
            if (telefoneInput && telefoneInput.value) {
                telefoneInput.value = telefoneInput.value.replace(/\D/g, '');
            }
        });
    });
});