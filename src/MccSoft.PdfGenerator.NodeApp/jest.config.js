const { pathsToModuleNameMapper } = require('ts-jest/utils');
const { compilerOptions } = require('./tsconfig');

module.exports = {
    preset: 'ts-jest',
    collectCoverageFrom: [
        "**/*.{ts,tsx}",
        "!**/node_modules/**"
      ],
    snapshotSerializers: ["jest-html"],
    testEnvironment: 'jsdom',
    testResultsProcessor: 'jest-teamcity-reporter',
    moduleNameMapper: {
        '^.+\\.(scss|css)$': 'identity-obj-proxy',
        ...pathsToModuleNameMapper(compilerOptions.paths, {
            prefix: '<rootDir>/src/'
        })
    },
    setupFiles: [
        './jest.setup.js'
    ]
};
