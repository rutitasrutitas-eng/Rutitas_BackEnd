import unittest
from pathlib import Path

import yaml
from jsonschema import Draft202012Validator
from openapi_spec_validator import validate

CONTRACT = (
    Path(__file__).resolve().parents[1]
    / "contracts"
    / "openapi-maps-001.yaml"
)

with CONTRACT.open(encoding="utf-8") as stream:
    API = yaml.safe_load(stream)


def validator_for(name):
    schemas = API["components"]["schemas"]
    return Draft202012Validator({
        "$schema": "https://json-schema.org/draft/2020-12/schema",
        "$ref": f"#/components/schemas/{name}",
        "components": {"schemas": schemas},
    })


class MapsContractTests(unittest.TestCase):

    def test_openapi_structure(self):
        validate(API)
        self.assertEqual(API["openapi"], "3.1.0")

    def test_exactly_four_operations(self):
        expected = {
            ("/maps/search", "get"),
            ("/maps/geocode", "get"),
            ("/maps/reverse-geocode", "get"),
            ("/maps/directions", "post"),
        }
        actual = {
            (path, method)
            for path, item in API["paths"].items()
            for method in item
            if method in {
                "get", "post", "put", "patch",
                "delete", "head", "options", "trace"
            }
        }
        self.assertEqual(expected, actual)

    def test_jwt_security_contract(self):
        bearer = API["components"]["securitySchemes"]["bearerAuth"]
        self.assertEqual(bearer["scheme"], "bearer")
        self.assertEqual(bearer["bearerFormat"], "JWT")
        for item in API["paths"].values():
            for operation in item.values():
                self.assertEqual(
                    operation.get("security", API["security"]),
                    [{"bearerAuth": []}],
                )

    def test_valid_coordinate(self):
        validator_for("Coordinate").validate({
            "latitude": 10.0,
            "longitude": -20.0,
        })

    def test_invalid_latitude(self):
        self.assertFalse(validator_for("Coordinate").is_valid({
            "latitude": 91,
            "longitude": 0,
        }))

    def test_invalid_longitude(self):
        self.assertFalse(validator_for("Coordinate").is_valid({
            "latitude": 0,
            "longitude": 181,
        }))

    def test_missing_coordinate(self):
        self.assertFalse(validator_for("Coordinate").is_valid({
            "latitude": 10,
        }))

    def test_mock_search_response(self):
        validator_for("Place").validate({
            "providerId": "synthetic-place-001",
            "label": "Lugar sintetico",
            "position": {
                "latitude": 10,
                "longitude": -20,
            },
        })

    def test_valid_directions_request(self):
        validator_for("DirectionsRequest").validate({
            "origin": {"latitude": 10, "longitude": -20},
            "destination": {"latitude": 11, "longitude": -21},
            "travelMode": "driving",
        })

    def test_invalid_travel_mode(self):
        self.assertFalse(validator_for("DirectionsRequest").is_valid({
            "origin": {"latitude": 10, "longitude": -20},
            "destination": {"latitude": 11, "longitude": -21},
            "travelMode": "invalid",
        }))

    def test_excessive_waypoints(self):
        self.assertFalse(validator_for("DirectionsRequest").is_valid({
            "origin": {"latitude": 10, "longitude": -20},
            "destination": {"latitude": 11, "longitude": -21},
            "travelMode": "driving",
            "waypoints": [
                {"latitude": 10, "longitude": -20}
                for _ in range(9)
            ],
        }))

    def test_mock_directions_response(self):
        validator_for("DirectionsResponse").validate({
            "origin": {"latitude": 10, "longitude": -20},
            "destination": {"latitude": 11, "longitude": -21},
            "distanceMeters": 12000,
            "durationSeconds": 900,
            "geometry": {
                "type": "LineString",
                "coordinates": [[-20, 10], [-21, 11]],
            },
            "provider": "azure-maps",
            "calculatedAtUtc": "2026-11-26T00:00:00Z",
        })

    def test_invalid_geojson(self):
        self.assertFalse(validator_for("LineString").is_valid({
            "type": "LineString",
            "coordinates": [[-20, 100], [-21, 11]],
        }))

    def test_required_error_responses(self):
        required = {
            "400", "401", "403", "429",
            "502", "503", "504",
        }
        for path, item in API["paths"].items():
            for method, operation in item.items():
                with self.subTest(path=path, method=method):
                    self.assertTrue(
                        required.issubset(operation["responses"])
                    )


if __name__ == "__main__":
    unittest.main(verbosity=2)
