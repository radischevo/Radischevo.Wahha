var MvcValidation = function() {
    var initComplete = false;
    var applyRule = {
        "range": function(rule, min, max) {
            rule["range"] = [min, max];
        },
        "regularExpression": function(rule, pattern) {
            rule["regex"] = pattern;
        },
        "required": function(rule) {
            rule["required"] = true;
        },
        "stringLength": function(rule, length) {
            rule["maxlength"] = length;
        },
        "unknown": function(rule, type, parameters) {
            rule[type] = parameters;
        }
    };
    var createFieldRules = function(field) {
        var collection = field.rules;
        var rules = {};

        for (var i = 0, l = collection.length; i < l; ++i) {
            var rule = collection[i];
            switch (rule.ValidationType) {
                case "range":
                    applyRule.range(rules,
                        rule.Parameters["minimum"],
                        rule.Parameters["maximum"]);
                    break;
                case "regularExpression":
                    applyRule.regularExpression(rules,
                        rule.Parameters["pattern"]);
                    break;
                case "required":
                    applyRule.required(rules);
                    break;
                case "stringLength":
                    applyRule.stringLength(rules,
                        rule.Parameters["maximumLength"]);
                    break;
                default:
                    applyRule.unknown(rules,
                        rule.ValidationType, rule.Parameters);
                    break;
            }
        }
        return rules;
    };
    var createRules = function(fields) {
        var rules = {};
        for (var i = 0, l = fields.length; i < l; ++i) {
            var f = fields[i];
            rules[f.field] = createFieldRules(f);
        }
        return rules;
    };
    var createMessages = function(fields) {
        var messages = {};

        for (var i = 0, l = fields.length; i < l; ++i) {
            var field = fields[i];
            var fieldMessages = {};
            var rules = field.rules;

            for (var j = 0, c = rules.length; j < c; ++j) {
                var rule = rules[j];
                if (rule.ErrorMessage) {
                    var jQueryValidationType = rule.ValidationType;
                    switch (rule.ValidationType) {
                        case "regularExpression":
                            jQueryValidationType = "regex";
                            break;
                        case "stringLength":
                            jQueryValidationType = "maxlength";
                            break;
                    }
                    fieldMessages[jQueryValidationType] = rule.ErrorMessage;
                }
            }
            messages[field.field] = fieldMessages;
        }
        return messages;
    };
    var init = function() {
        if (!initComplete) {
            jQuery.validator.addMethod("regex", function(value, element, params) {
                return this.optional(element) || new RegExp(params).test(value);
            });
        }
        initComplete = true;
    };
    return {
        apply: function(options) {
            init();
            $(options.selector).validate({
                errorClass: "validation-error",
                errorElement: "span",
                errorPlacement: function(error, element) {
                    error.insertAfter(element);
                },
                messages: createMessages(options.fields),
                rules: createRules(options.fields),
                success: function(label) {
                    label.remove();
                }
            });
        }
    };
} ();
