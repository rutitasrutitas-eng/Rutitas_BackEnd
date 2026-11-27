import unittest
from pathlib import Path

import yaml
from jsonschema import Draft202012Validator, FormatChecker
from openapi_spec_validator import validate

CONTRACT = (
    Path(__file__).resolve().parents[1]
    / "contracts"
    / "openapi-content-safety-001.yaml"
)

with CONTRACT.open(encoding="utf-8") as stream:
    API = yaml.safe_load(stream)


def validator_for(name):
    return Draft202012Validator(
        {
            "$schema": "https://json-schema.org/draft/2020-12/schema",
            "$ref": f"#/components/schemas/{name}",
            "components": {
                "schemas": API["components"]["schemas"]
            },
        },
        format_checker=FormatChecker(),
    )


def valid_text():
    return {
        "context": "profile_description",
        "text": "Contenido sintetico de prueba",
    }


def valid_result():
    return {
        "moderationId": "00000000-0000-4000-8000-000000000001",
        "decision": "approved",
        "policyVersion": "candidate-001",
        "evaluatedAtUtc": "2026-11-26T00:00:00Z",
    }


class ContentSafetyContractTests(unittest.TestCase):

    def test_openapi_structure(self):
        validate(API)
        self.assertEqual(API["openapi"], "3.1.0")

    def test_exactly_two_operations(self):
        actual = {
            (path, method)
            for path, item in API["paths"].items()
            for method in item
            if method in {
                "get", "post", "put", "patch", "delete",
                "head", "options", "trace",
            }
        }
        self.assertEqual(actual, {
            ("/moderation/text", "post"),
            ("/moderation/image", "post"),
        })

    def test_jwt_security(self):
        bearer = API["components"]["securitySchemes"]["bearerAuth"]
        self.assertEqual(bearer["scheme"], "bearer")
        self.assertEqual(bearer["bearerFormat"], "JWT")

        for item in API["paths"].values():
            operation = item["post"]
            self.assertEqual(
                operation.get("security", API["security"]),
                [{"bearerAuth": []}],
            )

    def test_valid_text(self):
        validator_for("TextModerationRequest").validate(valid_text())

    def test_empty_text(self):
        data = valid_text()
        data["text"] = ""
        self.assertFalse(
            validator_for("TextModerationRequest").is_valid(data)
        )

    def test_whitespace_text(self):
        data = valid_text()
        data["text"] = "   "
        self.assertFalse(
            validator_for("TextModerationRequest").is_valid(data)
        )

    def test_text_length_limit(self):
        validator = validator_for("TextModerationRequest")
        data = valid_text()
        data["text"] = "a" * 2000
        self.assertTrue(validator.is_valid(data))
        data["text"] = "a" * 2001
        self.assertFalse(validator.is_valid(data))

    def test_invalid_text_context(self):
        data = valid_text()
        data["context"] = "profile_photo"
        self.assertFalse(
            validator_for("TextModerationRequest").is_valid(data)
        )

    def test_client_cannot_set_decision(self):
        validator = validator_for("TextModerationRequest")
        data = valid_text()
        data["decision"] = "approved"
        self.assertFalse(validator.is_valid(data))

    def test_client_cannot_set_owner(self):
        data = valid_text()
        data["userId"] = "synthetic-user"
        self.assertFalse(
            validator_for("TextModerationRequest").is_valid(data)
        )

    def test_image_schema(self):
        validator_for("ImageModerationRequest").validate({
            "context": "profile_photo",
            "image": "synthetic-binary-placeholder",
        })

    def test_image_missing_file(self):
        self.assertFalse(
            validator_for("ImageModerationRequest").is_valid({
                "context": "profile_photo",
            })
        )

    def test_image_url_not_allowed(self):
        self.assertFalse(
            validator_for("ImageModerationRequest").is_valid({
                "context": "profile_photo",
                "imageUrl": "https://example.invalid/image.png",
            })
        )

    def test_all_moderation_decisions(self):
        validator = validator_for("ModerationResult")
        for decision in ("approved", "rejected", "pending_review"):
            with self.subTest(decision=decision):
                data = valid_result()
                data["decision"] = decision
                validator.validate(data)

    def test_invalid_result(self):
        validator = validator_for("ModerationResult")
        data = valid_result()
        data["decision"] = "unknown"
        self.assertFalse(validator.is_valid(data))
        data = valid_result()
        data["evaluatedAtUtc"] = "invalid-date"
        self.assertFalse(validator.is_valid(data))

    def test_error_contract(self):
        required = {
            "400", "401", "403", "413", "415",
            "429", "502", "503", "504",
        }
        responses = API["components"]["responses"]

        for item in API["paths"].values():
            declared = item["post"]["responses"]
            self.assertTrue(required.issubset(declared))

            for status in required:
                name = declared[status]["$ref"].split("/")[-1]
                self.assertIn(
                    "application/problem+json",
                    responses[name]["content"],
                )


if __name__ == "__main__":
    unittest.main(verbosity=2)
